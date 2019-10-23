/*
Copyright 2019, Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES 
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using System;
using DotNetNuke.Entities.Portals;

namespace Upendo.Libraries.FileSync.ScheduledJobs
{
    /// <summary>
    /// A scheduled job that can be managed in DNN.
    /// </summary>
    public class SyncFiles : SchedulerClient
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(SyncFiles));

        /// <summary>
        /// Gets things started...
        /// </summary>
        /// <param name="oItem"></param>
        public SyncFiles(ScheduleHistoryItem oItem) : base()
        {
            ScheduleHistoryItem = oItem;
        }

        /// <summary>
        /// This method does all of the real work.
        /// </summary>
        public override void DoWork()
        {
            try
            {
                // Perform required items for logging
                Progressing();

                ScheduleHistoryItem.AddLogNote("SyncFiles Starting");
                Logger.Debug("SyncFiles Starting");

				//
                SynchronizeFiles();
				//

                ScheduleHistoryItem.AddLogNote("SyncFiles Completed");
                Logger.Debug("SyncFiles Completed");

                // Show success
                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote("Exception:: " + ex.ToString());
                Exceptions.LogException(ex);
            }
        }

        private void SynchronizeFiles()
        {
            var ctlPortal = new PortalController();
            var portals = ctlPortal.GetPortals();

            foreach (PortalInfo portal in portals)
            {
                SyncForPortal(portal.PortalID);
            }
        }

        private void SyncForPortal(int portalId)
        {
            try
            {
                FolderManager.Instance.Synchronize(portalId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ScheduleHistoryItem.AddLogNote("Exception:: " + ex.ToString());
                Exceptions.LogException(ex);
            }
        }
    }
}