// <copyright file="Achievements.cs" company="Jan Ivar Z. Carlsen, Sindri Jóelsson">
// Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace CloudOnce
{
    using System.Collections.Generic;
    using Internal;

    /// <summary>
    /// Provides access to achievements registered via the CloudOnce Editor.
    /// This file was automatically generated by CloudOnce. Do not edit.
    /// </summary>
    public static class Achievements
    {
        public static readonly UnifiedAchievement[] All =
        {
        };

        public static string GetPlatformID(string internalId)
        {
            return s_achievementDictionary.ContainsKey(internalId)
                ? s_achievementDictionary[internalId].ID
                : string.Empty;
        }

        private static readonly Dictionary<string, UnifiedAchievement> s_achievementDictionary = new Dictionary<string, UnifiedAchievement>
        {

        };
    }
}
