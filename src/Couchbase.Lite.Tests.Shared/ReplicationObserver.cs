//
// ReplicationTest.cs
//
// Author:
//     Zachary Gramana  <zack@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc
// Copyright (c) 2014 .NET Foundation
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
// Copyright (c) 2014 Couchbase, Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
// except in compliance with the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions
// and limitations under the License.
//

using System;
using Couchbase.Lite;
using Couchbase.Lite.Util;
using System.Threading;

namespace Couchbase.Lite
{
    public class ReplicationObserver 
    {
        const string Tag = "ReplicationObserver";

        private bool replicationFinished = false;
        private readonly CountdownEvent doneSignal;

        internal ReplicationObserver(CountdownEvent doneSignal)
        {
            this.doneSignal = doneSignal;
        }

        public void Changed(object sender, ReplicationChangeEventArgs args)
        {
            if (replicationFinished) {
                return;
            }

            var replicator = args;
            LiteTestCase.WriteDebug(replicator + " changed: " + replicator.CompletedChangesCount + " / " + replicator.ChangesCount);

            if (replicator.CompletedChangesCount < 0) {
                var msg = replicator + ": replicator.CompletedChangesCount < 0";
                Console.WriteLine(msg);
                throw new Exception(msg);
            }

            if (replicator.ChangesCount < 0) {
                var msg = replicator + ": replicator.ChangesCount < 0";
                Console.WriteLine(msg);
                throw new Exception(msg);
            }

            if (replicator.CompletedChangesCount > replicator.ChangesCount) {
                var msgStr = "replicator.CompletedChangesCount : " + replicator.CompletedChangesCount +
                             " > replicator.ChangesCount : " + replicator.ChangesCount;

                Console.WriteLine(msgStr);
                throw new Exception(msgStr);
            }

            if (args.Status == ReplicationStatus.Stopped || args.Status == ReplicationStatus.Idle) {
                this.replicationFinished = true;
                string msg = "ReplicationFinishedObserver.changed called, set replicationFinished to true";
                LiteTestCase.WriteDebug(msg);
                if(doneSignal.CurrentCount > 0) {
                    doneSignal.Signal();
                }

                args.Source.Changed -= Changed;
            } else {
                string msg = string.Format("ReplicationFinishedObserver.changed called, but replicator still running, so ignore it");
                LiteTestCase.WriteDebug(msg);
            }
        }

        internal virtual bool IsReplicationFinished()
        {
            return this.replicationFinished;
        }
    }

}