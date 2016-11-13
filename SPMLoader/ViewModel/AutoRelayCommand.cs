using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace SPMLoader.ViewModel
{
    public class AutoRelayCommand : RelayCommand, IDisposable
    {
        private ISet<string> properties;

        public AutoRelayCommand(Action execute)
            : base(execute)
        {
            this.Initialize();
        }

        public AutoRelayCommand(Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            Messenger.Default.Register<PropertyChangedMessageBase>(this, true, (property) =>
            {
                if (properties != null && properties.Contains(property.PropertyName))
                    this.RaiseCanExecuteChanged();
            });
        }

        public void DependsOn<T>(Expression<Func<T>> propertyExpression)
        {
            if (properties == null)
                properties = new HashSet<string>();

            properties.Add(this.GetPropertyName(propertyExpression));
        }

        private string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Invalid argument", "propertyExpression");

            var property = body.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Argument is not a property",
                    "propertyExpression");

            return property.Name;
        }

        public void Dispose()
        {
            Messenger.Default.Unregister(this);
        }
    }
}
