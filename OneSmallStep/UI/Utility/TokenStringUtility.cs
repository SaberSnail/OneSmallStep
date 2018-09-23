using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using GoldenAnvil.Utility;
using OneSmallStep.ECS;

namespace OneSmallStep.UI.Utility
{
	public static class TokenStringUtility
	{
		public static readonly RoutedCommand GoToEntityCommand = new RoutedCommand("GoToEntityCommand", typeof(TokenStringUtility));

		public static string GetString(string tokenString, EntityManager entityManager)
		{
			var builder = new StringBuilder();
			int currentIndex = 0;

			var entityLookup = entityManager?.DisplayEntityLookup;

			if (entityLookup != null)
			{
				var matches = s_tokenRegex.Matches(tokenString);
				foreach (var match in matches.Cast<Match>())
				{
					if (match.Index > currentIndex)
						builder.Append(tokenString.Substring(currentIndex, match.Index - currentIndex));

					string hyperlinkText = null;
					var token = match.Groups[1].Captures[0].ToString();
					var tokenParts = s_tokenSplitRegex.Split(token);
					if (tokenParts.Length > 2)
					{
						Entity entity = null;
						if (int.TryParse(tokenParts[0], out int idValue))
						{
							var entityId = new EntityId(idValue);
							entity = entityLookup.GetEntity(entityId);
						}

						ComponentBase component = null;
						if (entity != null)
							component = entity.GetOptionalComponentByName(tokenParts[1]);

						if (component != null)
						{
							hyperlinkText = FollowPropertyPath(component, tokenParts, 2)?.ToString();
							builder.Append(hyperlinkText);
						}
					}

					if (hyperlinkText == null)
						builder.Append(token);

					currentIndex = match.Index + match.Length;
				}
			}

			if (currentIndex < tokenString.Length)
				builder.Append(tokenString.Substring(currentIndex));

			return builder.ToString();
		}

		public static IEnumerable<Inline> GetInlines(string tokenString, EntityManager entityManager)
		{
			int currentIndex = 0;

			var entityLookup = entityManager?.DisplayEntityLookup;

			if (entityLookup != null)
			{
				var matches = s_tokenRegex.Matches(tokenString);
				foreach (var match in matches.Cast<Match>())
				{
					if (match.Index > currentIndex)
						yield return new Run(tokenString.Substring(currentIndex, match.Index - currentIndex));

					string hyperlinkText = null;
					var token = match.Groups[1].Captures[0].ToString();
					var tokenParts = s_tokenSplitRegex.Split(token);
					if (tokenParts.Length > 2)
					{
						EntityId? entityId = null;
						Entity entity = null;
						if (int.TryParse(tokenParts[0], out int idValue))
						{
							entityId = new EntityId(idValue);
							entity = entityLookup.GetEntity(entityId.Value);
						}

						ComponentBase component = null;
						if (entity != null)
							component = entity.GetOptionalComponentByName(tokenParts[1]);

						if (component != null)
						{
							hyperlinkText = FollowPropertyPath(component, tokenParts, 2)?.ToString();
							var hyperlinkRun = new Run(hyperlinkText);
							var hyperlink = new Hyperlink(hyperlinkRun);
							hyperlink.Command = GoToEntityCommand;
							hyperlink.CommandParameter = entityId.Value;
							yield return hyperlink;
						}
					}

					if (hyperlinkText == null)
						yield return new Run(token);

					currentIndex = match.Index + match.Length;
				}
			}

			if (currentIndex < tokenString.Length)
				yield return new Run(tokenString.Substring(currentIndex));
		}

		public static readonly DependencyProperty TokenStringProperty = DependencyProperty.RegisterAttached("TokenString", typeof(string), typeof(TokenStringUtility), new UIPropertyMetadata(null, OnTokenStringChanged));

		public static string GetTokenString(DependencyObject d)
		{
			return (string) d.GetValue(TokenStringProperty);
		}

		public static void SetTokenString(DependencyObject d, string value)
		{
			d.SetValue(TokenStringProperty, value);
		}

		private static void OnTokenStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBlock textBlock)
			{
				textBlock.ClearValue(TextBlock.TextProperty);

				var tokenString = GetTokenString(textBlock);
				var entityManager = ((App) Application.Current).AppModel.GameData?.EntityManager;

				IEnumerable<Inline> inlines = entityManager != null ? GetInlines(tokenString, entityManager) : EnumerableUtility.Enumerate(new Run(tokenString));

				textBlock.Inlines.AddRange(inlines);
			}
		}

		private static object FollowPropertyPath(object value, string[] path, int startingIndex)
		{
			Type currentType = value.GetType();

			foreach (string propertyName in path.Skip(startingIndex))
			{
				PropertyInfo property = currentType.GetProperty(propertyName);
				value = property.GetValue(value, null);
				currentType = property.PropertyType;
			}

			return value;
		}

		private static readonly Regex s_tokenRegex = new Regex(@"(?<!\\)[{](.*?)(?<!\\)[}]");
		private static readonly Regex s_tokenSplitRegex = new Regex(@"(?<!\\)\.");
	}
}
