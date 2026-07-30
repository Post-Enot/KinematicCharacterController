using UnityEngine;

namespace PostEnot.KinematicCharacterController
{
    [CreateAssetMenu(fileName = "KCC_Settings", menuName = "PostEnot/KCC Settings")]
    public sealed class KCC_Settings : ScriptableObject
    {
        #region Inspector
        [Tooltip("Определяет, выполняется ли симуляция автоматически. Если true, симуляция происходит в FixedUpdate.")]
        [SerializeField]
        private bool autoSimulation = true;

        [Tooltip("Должна ли обрабатываться интерполяция персонажей и PhysicsMover'ов.")]
        [SerializeField]
        private bool interpolate = true;

        [Tooltip(
            "Начальная ёмкость списка моторов в системе (при необходимости список расширится автоматически, " +
            "но установка достаточно большой начальной ёмкости помогает избежать выделения памяти во время выполнения).\n" +
            "Если значение <= 0, будет использована ёмкость по умолчанию (как при вызове new List<>()).\n" +
            "Рекомендуется указывать значение, равное степени двойки.")]
        [SerializeField]
        [Min(-1)]
        private int motorsListInitialCapacity = 100;

        [Tooltip(
            "Начальная ёмкость списка моверов в системе (при необходимости список расширится автоматически, " +
            "но установка достаточно большой начальной ёмкости помогает избежать выделения памяти во время выполнения).\n" +
            "Если значение <= 0, будет использована ёмкость по умолчанию (как при вызове new List<>()).\n" +
            "Рекомендуется указывать значение, равное степени двойки.")]
        [SerializeField]
        [Min(-1)]
        private int moversListInitialCapacity = 100;
        #endregion

        /// <summary>
        /// Определяет, выполняется ли симуляция автоматически.
        /// Если true, симуляция происходит в FixedUpdate.
        /// </summary>
        public bool AutoSimulation
        {
            get => autoSimulation;
            set => autoSimulation = value;
        }

        /// <summary>
        /// Должна ли обрабатываться интерполяция персонажей и <see cref="PhysicsMover"/>'ов.
        /// </summary>
        public bool Interpolate
        {
            get => interpolate;
            set => interpolate = value;
        }

        /// <summary>
        /// Начальная ёмкость списка моторов в системе.
        /// При необходимости список расширится автоматически, но установка достаточно большой начальной ёмкости
        /// помогает избежать выделения памяти во время выполнения.
        /// Если значение меньше или равно 0, будет использована ёмкость по умолчанию (как при вызове new List()).
        /// Рекомендуется указывать значение, равное степени двойки.
        /// </summary>
        public int MotorsListInitialCapacity
        {
            get => motorsListInitialCapacity;
            set => motorsListInitialCapacity = value;
        }

        /// <summary>
        /// Начальная ёмкость списка моверов в системе.
        /// При необходимости список расширится автоматически, но установка достаточно большой начальной ёмкости
        /// помогает избежать выделения памяти во время выполнения.
        /// Если значение меньше или равно 0, будет использована ёмкость по умолчанию (как при вызове new List()).
        /// Рекомендуется указывать значение, равное степени двойки.
        /// </summary>
        public int MoversListInitialCapacity
        {
            get => moversListInitialCapacity;
            set => moversListInitialCapacity = value;
        }
    }
}
