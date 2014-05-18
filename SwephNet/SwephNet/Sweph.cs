﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwephNet
{

    /// <summary>
    /// Swiss Ephmeris context
    /// </summary>
    public class Sweph : 
        IDisposable,
        IStreamProvider
    {
        private IDependencyContainer _Dependencies;

        #region Ctors & Dest

        /// <summary>
        /// Create a new engine
        /// </summary>
        public Sweph() {
        }

        /// <summary>
        /// Internal release resources
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_Dependencies != null) {
                    _Dependencies.Dispose();
                }
                _Dependencies = null;
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }

        #endregion

        #region Dependency container

        /// <summary>
        /// Get the current container
        /// </summary>
        protected IDependencyContainer GetDependencies() {
            if (_Dependencies == null) {
                _Dependencies = CreateDependencyContainer();
                BuildDependencies(_Dependencies);
            }
            return _Dependencies;
        }

        /// <summary>
        /// Create a new container
        /// </summary>
        protected virtual IDependencyContainer CreateDependencyContainer() {
            return new Dependency.SimpleContainer();
        }

        /// <summary>
        /// Create all dependencies
        /// </summary>
        protected virtual void BuildDependencies(IDependencyContainer container) {
            // Register default type
            container.RegisterInstance(this);
            container.RegisterInstance<IStreamProvider>(this);
            container.RegisterInstance<IDependencyContainer>(container);
            // Register engines types
            container.Register<SweDate, SweDate>();
        }

        #endregion

        #region File management

        /// <summary>
        /// Load a file
        /// </summary>
        public System.IO.Stream LoadFile(string filename)
        {
            var h = OnLoadFile;
            if (h != null)
            {
                var e = new LoadFileEventArgs(filename);
                h(this, e);
                return e.File;
            }
            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Dependency container
        /// </summary>
        public IDependencyContainer Dependencies { get { return GetDependencies(); } }

        /// <summary>
        /// Date engine
        /// </summary>
        public SweDate Date {
            get { return Dependencies.Resolve<SweDate>(); }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when a file is required
        /// </summary>
        public event EventHandler<LoadFileEventArgs> OnLoadFile;

        #endregion

    }

}
