using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Diagnostics;

using System.Diagnostics.PerformanceData;

namespace Core.Diagnostics
{
    public class MemoryProfiler : DiagnosticComponent
    {
        private long ramInUse;
        private long memoryPaged;

        private long peakRamUsage;

        public MemoryProfiler(DiagnosticsManager diagnostics) : base(diagnostics)
        {
        }
        public override string ComponentID
        {
            get { return "Memory Profiler"; }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ramInUse = DiagnosticManager.AppDomainProcess.WorkingSet64;
            memoryPaged = DiagnosticManager.AppDomainProcess.VirtualMemorySize64;

            peakRamUsage = DiagnosticManager.AppDomainProcess.PeakWorkingSet64;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }



        public override string[] GetValues()
        {
            return new String[] { ramInUse.ToString(), memoryPaged.ToString(), peakRamUsage.ToString() };
        }
    }
}
