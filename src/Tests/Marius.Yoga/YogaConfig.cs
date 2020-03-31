/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * Copyright (c) 2018-present, Marius Klimantavičius
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Marius.Yoga
{
    public class YogaConfig
    {
        private bool[] _experimentalFeatures = new bool[] { false };
        private bool _useWebDefaults;
        private bool _useLegacyStretchBehaviour;
        private bool _shouldDiffLayoutWithoutLegacyStretchBehaviour;
        private float _pointScaleFactor = 1.0F;
        private YogaNodeCloned _onNodeCloned;

        public bool[] ExperimentalFeatures
        {
            get { return _experimentalFeatures; }
            set { _experimentalFeatures = value; }
        }

        public bool UseWebDefaults
        {
            get { return _useWebDefaults; }
            set { _useWebDefaults = value; }
        }

        public bool UseLegacyStretchBehaviour
        {
            get { return _useLegacyStretchBehaviour; }
            set { _useLegacyStretchBehaviour = value; }
        }

        public bool ShouldDiffLayoutWithoutLegacyStretchBehaviour
        {
            get { return _shouldDiffLayoutWithoutLegacyStretchBehaviour; }
            set { _shouldDiffLayoutWithoutLegacyStretchBehaviour = value; }
        }

        public float PointScaleFactor
        {
            get { return _pointScaleFactor; }
            set { _pointScaleFactor = value; }
        }

        public YogaNodeCloned OnNodeCloned
        {
            get { return _onNodeCloned; }
            set { _onNodeCloned = value; }
        }

        public YogaConfig()
        {
        }

        public YogaConfig(YogaConfig oldConfig)
        {
            ExperimentalFeatures = new bool[oldConfig.ExperimentalFeatures.Length];
            Array.Copy(oldConfig.ExperimentalFeatures, ExperimentalFeatures, ExperimentalFeatures.Length);

            UseWebDefaults = oldConfig.UseWebDefaults;
            UseLegacyStretchBehaviour = oldConfig.UseLegacyStretchBehaviour;
            ShouldDiffLayoutWithoutLegacyStretchBehaviour = oldConfig.ShouldDiffLayoutWithoutLegacyStretchBehaviour;
            PointScaleFactor = oldConfig.PointScaleFactor;
            OnNodeCloned = oldConfig.OnNodeCloned;
        }

        public bool IsExperimentalFeatureEnabled(YogaExperimentalFeature feature)
        {
            return ExperimentalFeatures[(int)feature];
        }

        public YogaConfig DeepClone()
        {
            return new YogaConfig(this);
        }
    };
}
