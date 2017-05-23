// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Orckestra.AspNet.Roslyn
{
    internal class AssemblyMetadataCache
    {
        private const long MaxInactiveTicksAllowed = 120_0000000L; // 120 seconds

        private const long MillisecondsInOneMinutes = 60000L;

        private static readonly AssemblyMetadataCache _instance = new AssemblyMetadataCache();

        private readonly ConcurrentDictionary<string, AssemblyMetadata> _dictionary;

        private DateTime lastActiveTime;

        private volatile Timer timer;

        private readonly object _lockObject;

        private AssemblyMetadataCache()
        {
            this._dictionary = new ConcurrentDictionary<string, AssemblyMetadata>();
            this._lockObject = new object();
            this.Activate();
        }

        public static AssemblyMetadataCache GetInstance()
        {
            return AssemblyMetadataCache._instance;
        }

        public AssemblyMetadata GetOrAdd(string key, Func<string, AssemblyMetadata> func)
        {
            this.Activate();
            //this.StartTimer();
            return this._dictionary.GetOrAdd(key, func);
        }

        private void Activate()
        {
            this.lastActiveTime = DateTime.Now;
        }

        private void StartTimer()
        {
            if (this.timer == null)
            {
                lock (this._lockObject)
                {
                    if (this.timer == null)
                    {
                        this.timer = new Timer(this.ClearIfInactive, null, MillisecondsInOneMinutes, MillisecondsInOneMinutes);
                    }
                }
            }
        }

        private bool IsActive()
        {
            return DateTime.Now.Ticks - this.lastActiveTime.Ticks > MaxInactiveTicksAllowed;
        }

        private void ClearIfInactive(object state)
        {
            if (!this.IsActive())
            {
                Timer timer = this.timer;
                if (Interlocked.CompareExchange<Timer>(ref this.timer, null, timer) == timer && timer != null)
                {
                    timer.Dispose();
                }

                ICollection<string> keys = this._dictionary.Keys;
                foreach (string current in keys)
                {
                    if (this._dictionary.TryRemove(current, out AssemblyMetadata assemblyMetadata))
                    {
                        try
                        {
                            assemblyMetadata?.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
    }
}
