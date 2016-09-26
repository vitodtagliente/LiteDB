using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LiteDB.Forms
{
    public class LiteFormAdapter<T> where T : LiteModel
    {
        Dictionary<string, Control> binding;

        public LiteFormAdapter()
        {
            binding = new Dictionary<string, Control>();
        }

        public void BindControl(string name, Control control)
        {
            binding.Add(name, control);
        }

        public void Read(T model)
        {
            foreach(var bind in binding)
            {
                var type = model.GetType();
                var field = type.GetField(bind.Key);
                if (field == null) continue;

                bind.Value.Text = field.GetValue(model).ToString();
            }
        }

        public T Write()
        {
            var instance = Activator.CreateInstance<T>();

            foreach(var bind in binding)
            {
                var field = instance.GetType().GetField(bind.Key);
                if (field == null)
                    continue;

                var fieldType = field.FieldType;
                Type converTo = Nullable.GetUnderlyingType(fieldType) ?? fieldType;

                field.SetValue(instance, Convert.ChangeType(bind.Value.Text, converTo));
            }

            return instance;
        }
    }
}
