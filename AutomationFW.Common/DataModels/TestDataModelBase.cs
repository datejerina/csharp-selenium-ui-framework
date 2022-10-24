using AutomationFW.Common.Helpers;
using System;
using System.Collections.Generic;

namespace AutomationFW.Common.DataModels
{
    public abstract class TestDataModelBase : ScenarioDataContextBase
    {
        public bool? HasBeenUsedUp { get; set; }
    }
}
