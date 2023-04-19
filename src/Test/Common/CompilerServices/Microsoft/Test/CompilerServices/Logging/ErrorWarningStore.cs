using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.CompilerServices.Logging
{
    public class ErrorWarningStore
    {
        private BuildStatusCollection errors;
        private BuildStatusCollection warnings;

        public ErrorWarningStore() : this (null, null)
        {  }

        public ErrorWarningStore(List<BuildError> errors, List<BuildWarning> warnings)
        {
            this.errors = new BuildStatusCollection();
            this.warnings = new BuildStatusCollection();

            if (errors != null)
            {
                foreach (BuildError buildError in errors)
                {
                    this.errors.Add(buildError);
                }
            }
            if (warnings != null)
            {
                foreach (BuildWarning buildWarning in warnings)
                {
                    this.warnings.Add(buildWarning);
                }
            }
        }

        public BuildStatusCollection Errors
        {
            get { return errors; }
        }

        public BuildStatusCollection Warnings
        {
            get { return warnings; }
        }

        public void Clear()
        {
            errors.Clear();
            warnings.Clear();
        }
    }
}
