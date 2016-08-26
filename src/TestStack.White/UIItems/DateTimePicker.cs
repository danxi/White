using System;
using System.Windows.Automation;
using TestStack.White.Configuration;
using TestStack.White.UIItems.Actions;
using TestStack.White.WindowsAPI;

namespace TestStack.White.UIItems
{
    public class DateTimePicker : UIItem
    {
        protected DateTimePicker() {}
        public DateTimePicker(AutomationElement automationElement, IActionListener actionListener) : base(automationElement, actionListener) {}

        public virtual DateTime? Date
        {
            get
            {
                var property = (string) Property(ValuePattern.ValueProperty);
                if (string.IsNullOrEmpty(property))
                    return null;
                return DateTime.Parse(property);
            }
            set
            {
                SetDate(value, CoreAppXmlConfiguration.Instance.DefaultDateFormat);
            }
        }

        public virtual void SetDate(DateTime? dateTime, DateFormat dateFormat)
        {
            if (dateTime == null)
            {
                Logger.Warn("DateTime cannot be null, value will not be set");
                return;
            }

            /*
            * Yishun:  26/08/2016 - For Winform Datepicker with min & Max limit, sometimes numbers will not be
            * truely entered, as the number out of it's range.  
            * Here we use the Up (↑）and down(↓) keys to move year, month and days.
            */
            if (Date != null)
            {
                KeyboardInput.SpecialKeys upOrDownKey;
                int diff = 0;
                
                for (int datePart = 2; datePart >= 0; datePart--)
                {
                    //move left to highligh year first(default Day is highlighted)
                    keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.LEFT, actionListener);
                    diff = dateFormat.DisplayValue(dateTime.Value, datePart) - dateFormat.DisplayValue(Date.Value, datePart);
                    upOrDownKey = diff > 0 ? KeyboardInput.SpecialKeys.UP : KeyboardInput.SpecialKeys.DOWN;
                    for (int i = 0; i < Math.Abs(diff); i++) keyboard.PressSpecialKey(upOrDownKey, actionListener);
                }
                
            }
            else
            {
                keyboard.Send(dateFormat.DisplayValue(dateTime.Value, 0).ToString(), actionListener);
                keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RIGHT, actionListener);
                keyboard.Send(dateFormat.DisplayValue(dateTime.Value, 1).ToString(), actionListener);
                keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RIGHT, actionListener);
                keyboard.Send(dateFormat.DisplayValue(dateTime.Value, 2).ToString(), actionListener);
                keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RIGHT, actionListener);
            }

        }
    }
}