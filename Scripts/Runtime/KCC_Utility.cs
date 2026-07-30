using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    /// <summary>
    /// Предоставляет утилитарные методы и константы для работы с контроллером персонажа.
    /// </summary>
    public static class KCC_Utility
    {
        /// <summary>
        /// Минимально необходимая разница между радиусом и высотой для корректной работы контроллера.
        /// </summary>
        public const float MinClearense = 0.01f;
        /// <summary>
        /// Минимально допустимый радиус контроллера персонажа.
        /// </summary>
        public const float MinRadius = 0.001f;
        /// <summary>
        /// Минимально допустимая высота контроллера персонажа.
        /// </summary>
        public const float MinHeight = (MinRadius * 2.0f) + MinClearense;

        /// <summary>
        /// Корректирует высоту и радиус контроллера персонажа для обеспечения безопасных значений;
        /// при корректировке предпочтение отдаётся сохранению исходного радиуса и увеличению высоты.
        /// </summary>
        /// <param name="height">Высота контроллера.</param>
        /// <param name="radius">Радиус контроллера.</param>
        public static void CalculateSafeCharacterDimensions(ref float height, ref float radius)
        {
            radius = Mathf.Max(radius, MinRadius);
            height = Mathf.Max(height, MinHeight);
            float minSafeHeight = (radius * 2.0f) + MinClearense;
            height = Mathf.Max(height, minSafeHeight);
            float maxSafeRadius = (height - MinClearense) / 2.0f;
            radius = Mathf.Min(radius, maxSafeRadius);
        }

        /// <summary>
        /// Рассчитывает безопасную высоту контроллера на основе заданных значений; 
        /// при рассчёте предпочтение отдаётся сохранению исходного радиуса и увеличению высоты.
        /// </summary>
        /// <param name="height">Исходное значение высоты.</param>
        /// <param name="radius">Исходное значение радиуса.</param>
        /// <returns>
        /// Безопасное значение высоты.
        /// <br/>
        /// Если рассчитанная минимальная безопасная высота превышает заданное значение <paramref name="height"/>,
        /// возвращается минимальная безопасная высота.
        /// <br/>
        /// В противном случае возвращается значение <paramref name="height"/> (после проверки на минимальное значение).
        /// </returns>
        public static float CalculateSafeCharacterHeight(float height, float radius)
        {
            radius = Mathf.Max(radius, MinRadius);
            height = Mathf.Max(height, MinHeight);
            float minSafeHeight = (radius * 2.0f) + MinClearense;
            return Mathf.Max(height, minSafeHeight);
        }

        /// <summary>
        /// Рассчитывает безопасный радиус контроллера на основе заданных значений;
        /// при рассчёте предпочтение отдаётся сохранению исходной высоты и уменьшению радиуса.
        /// </summary>
        /// <param name="height">Исходное значение высоты.</param>
        /// <param name="radius">Исходное значение радиуса.</param>
        /// <returns>
        /// Безопасное значение радиуса.
        /// <br/>
        /// Если рассчитанный максимальный безопасный радиус меньше заданного значения <paramref name="radius"/>,
        /// возвращается максимальный безопасный радиус.
        /// <br/>
        /// В противном случае возвращается значение <paramref name="radius"/> (после проверки на минимальное значение).
        /// </returns>
        public static float CalculateSafeCharacterRadius(float height, float radius)
        {
            radius = Mathf.Max(radius, MinRadius);
            height = Mathf.Max(height, MinHeight);
            float maxSafeRadius = (height - MinClearense) / 2.0f;
            return Mathf.Min(radius, maxSafeRadius);
        }
    }
}
