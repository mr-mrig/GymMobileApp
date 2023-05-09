using System;
using TestApp.Models.Base;
using TestApp.Services.Utils;

namespace TestApp.Models.TrainingDomain
{
    public class TrainingEffort : BaseModel
    {


        #region Backing fields

        private float? _effort;
        private TrainingEffortType _effortType;
        #endregion

        public TrainingEffortType EffortType
        {
            get => _effortType;
            set => Set(ref _effortType, value);
        }

        public float? Effort
        {
            get => _effort;
            set => Set(ref _effort, value);
        }



        public TrainingEffort(float effortValue, uint effortTypeId)
        {
            Effort = effortValue;
            EffortType = new TrainingEffortType(effortTypeId);
        }


        /// <summary>
        /// Check whether the effort is expressed as Intensity Percentage
        /// </summary>
        /// <returns>True if Intensity Percentage</returns>
        public bool IsIntensityPercentage() => EffortType == TrainingEffortType.IntensityPercentage;


        /// <summary>
        /// Check whether the effort is expressed as RM
        /// </summary>
        /// <returns>True if RM</returns>
        public bool IsRM() => EffortType == TrainingEffortType.RM;


        /// <summary>
        /// Check whether the effort is expressed as RPE
        /// </summary>
        /// <returns>True if RPE</returns>
        public bool IsRPE() => EffortType == TrainingEffortType.RPE;



        #region Conversions

        /// <summary>
        /// Convert this effort value to RM, without rounding to the 0-th decimal
        /// To be used when performing intermediate conversions in order to keep the highest precision possible
        /// </summary>
        /// <param name="targetReps">The target repetitions, needed for RPE effort types only</param>
        /// <exception cref="ArgumentException">Thrown if repetitions are invalid</exception>
        /// <returns>The RM</returns>
        private float ToRmExact(int? targetReps = null)
        {
            if (!Effort.HasValue)
                throw new ArgumentException($"Cannot perform conversion since no effort has been specified.", nameof(targetReps));

            // From Intenisty Percentage
            if (EffortType == TrainingEffortType.IntensityPercentage)
            {
                //float input = Math.Min(Effort.Value, TrainingEffortType.OneRMIntensityPercentage);
                float input = Math.Min(Effort.Value, 100f);

                return (float)(
                    (324.206809067032 - 18.0137586362208 * input + 0.722425494099458 * Math.Pow(input, 2) - 0.018674659779516 * Math.Pow(input, 3)
                    + 0.00025787003728422 * Math.Pow(input, 4) - 1.65095582844966E-06 * Math.Pow(input, 5) + 2.75225269851 * Math.Pow(10, -9) * Math.Pow(input, 6)
                    + 8.99097867 * Math.Pow(10, -12) * Math.Pow(input, 7)));
            }

            // From RPE
            if (EffortType ==TrainingEffortType.RPE)
            {
                if (targetReps == null)
                    throw new ArgumentException($"Invalid repetions when converting from RPE.", nameof(targetReps));

                if (targetReps == Services.Utils.AppEnvironment.WsAmrapValue)
                    throw new ArgumentException($"Cannot convert from RPE when the reps are AMRAP.", nameof(targetReps));
                else
                    //return targetReps.Value + (TrainingEffortTypeEnum.AMRAPAsRPE - Value);
                    return targetReps.Value + (10 - Effort.Value);
            }
            return Effort.Value;
        }


        /// <summary>
        /// Convert this effort value to the RM expression
        /// </summary>
        /// <param name="targetReps">The target repetitions, needed for RPE effort types only</param>
        /// <exception cref="ArgumentException">Thrown if repetitions are invalid</exception>
        /// <returns>The new TrainingEffortValue instance</returns>
        public float ToRm(int? targetReps = null)
        {
            if (!Effort.HasValue)
                throw new ArgumentException($"Cannot perform conversion since no effort has been specified.", nameof(targetReps));

            // From Intenisty Percentage
            if (EffortType == TrainingEffortType.IntensityPercentage)
            {
                //float saturatedInput = Math.Min(Effort, TrainingEffortTypeEnum.OneRMIntensityPercentage);   // Truncate to 100%
                float saturatedInput = Math.Min(Effort.Value, 100);   // Truncate to 100%

                return (float)(324.206809067032 - 18.0137586362208 * saturatedInput + 0.722425494099458 * Math.Pow(saturatedInput, 2) - 0.018674659779516 * Math.Pow(saturatedInput, 3)
                    + 0.00025787003728422 * Math.Pow(saturatedInput, 4) - 1.65095582844966E-06 * Math.Pow(saturatedInput, 5) + 2.75225269851 * Math.Pow(10, -9) * Math.Pow(saturatedInput, 6)
                    + 8.99097867 * Math.Pow(10, -12) * Math.Pow(saturatedInput, 7));
            }

            // From RPE
            if (EffortType ==TrainingEffortType.RPE)
            {
                float saturatedInput = targetReps.Value == 1 ?
                    Math.Min(Effort.Value, 10) :                    // Cannot do more than 1RM -> Truncate to 10RPE
                    Effort.Value;

                if (targetReps == null)
                    throw new ArgumentException($"Invalid repetions when converting from RPE.", nameof(targetReps));

                if (targetReps == Services.Utils.AppEnvironment.WsAmrapValue)
                    throw new ArgumentException($"Cannot convert from RPE when the reps are AMRAP.", nameof(targetReps));
                else
                    //return (targetReps.Value + (TrainingEffortTypeEnum.AMRAPAsRPE - saturatedInput));    // Truncate
                    return targetReps.Value + (10 - saturatedInput);    // Truncate
            }
            return Effort.Value;
        }


        /// <summary>
        /// Convert this effort value to the Intensity Percentage expression
        /// </summary>
        /// <param name="targetReps">The target repetitions, needed for RPE effort types only</param>
        /// <returns>The new TrainingEffortValue instance</returns>
        public void ToIntensityPercentage(int? targetReps = null)
        {
            if (!Effort.HasValue)
                return;

            float converted = Effort.Value;

            // From RM
            if (EffortType == TrainingEffortType.RM)

                //converted = Math.Min(
                //    (float)Math.Round(0.4167 * Effort.Value - 14.2831 * Math.Pow(Effort.Value, 0.5) + 115.6122, 1),
                //    TrainingEffortTypeEnum.OneRMIntensityPercentage);
                converted = Math.Min(
                    (float)Math.Round(0.4167 * Effort.Value - 14.2831 * Math.Pow(Effort.Value, 0.5) + 115.6122, 1), 100);

            // From RPE
            if (EffortType ==TrainingEffortType.RPE)
            {
                float saturatedInput;

                if (targetReps == 1)
                    //saturatedInput = Math.Min(Effort.Value, TrainingEffortTypeEnum.AMRAPAsRPE);     // Cannot do more than 1RM -> Truncate to 10RPE
                    saturatedInput = Math.Min(Effort.Value, 10);     // Cannot do more than 1RM -> Truncate to 10RPE
                else
                    saturatedInput = Effort.Value;

                if (targetReps == null)
                    throw new ArgumentException($"Invalid repetions when converting from RPE.", nameof(targetReps));

                if (targetReps == Services.Utils.AppEnvironment.WsAmrapValue)
                    throw new ArgumentException($"Cannot convert from RPE when the reps are AMRAP.", nameof(targetReps));

                //int rmValue = (int)(targetReps.Value + (TrainingEffortTypeEnum.AMRAPAsRPE - saturatedInput));
                int rmValue = (int)(targetReps.Value + (10 - saturatedInput));

                converted =  (float)Math.Round(0.4167 * rmValue - 14.2831 * Math.Pow(rmValue, 0.5) + 115.6122, 1);
            }
            Effort = converted;
        }


        /// <summary>
        /// Convert this effort value to the RPE expression
        /// </summary>
        /// <param name="targetReps">The target repetitions, needed for RPE effort types only</param>
        /// <returns>The new TrainingEffortValue instance</returns>
        public void ToRPE(int? targetReps = null)
        {
            if (!Effort.HasValue)
                return;

            float converted = Effort.Value;
            float rmValue;

            if (targetReps == null)
                throw new ArgumentException($"Null repetions object when converting to RPE.", nameof(targetReps));

            if (targetReps == Services.Utils.AppEnvironment.WsAmrapValue)
                //converted = TrainingEffortTypeEnum.AMRAPAsRPE;
                converted = 10;

            // From Intenisty Percentage
            if (EffortType == TrainingEffortType.IntensityPercentage)
                rmValue = ToRmExact();     // Convert to RM firstB
            else
                rmValue = Effort.Value;

            // RM to RPE
            //Effort = Math.Max(TrainingEffortTypeEnum.AMRAPAsRPE - (rmValue - targetReps.Value), TrainingEffortTypeEnum.MinRPE);
            Effort = Math.Max(10 - (rmValue - targetReps.Value), TrainingEffortType.MinimumRPE);
        }

        #endregion
    }
}
