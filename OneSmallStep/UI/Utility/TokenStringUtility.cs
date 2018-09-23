using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using OneSmallStep.ECS;

namespace OneSmallStep.UI.Utility
{
	public static class TokenStringUtility
	{
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
				var matches = s_tokenRegex.Matches(tokenString);
				var inlines = GetInlinesForTokenStringWithMatches(tokenString, matches);

				textBlock.Inlines.AddRange(inlines);
			}
		}

		private static IEnumerable<Inline> GetInlinesForTokenStringWithMatches(string tokenString, MatchCollection matches)
		{
			int currentIndex = 0;

			var entityManager = ((App) Application.Current).AppModel.GameData?.EntityManager;
			var entityLookup = entityManager?.DisplayEntityLookup;

			if (entityLookup != null)
			{
				foreach (var match in matches.Cast<Match>())
				{
					if (match.Index > currentIndex)
						yield return new Run(tokenString.Substring(currentIndex, match.Index - currentIndex));

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
							var hyperlinkRun = new Run(hyperlinkText);
							yield return new Hyperlink(hyperlinkRun);
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
