using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Test_ImplementValidation.ViewModel
{
    public class MainViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();
        public bool HasErrors => Errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                return Errors[propertyName];
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }
        public void Validate(string PropertyName, object PropertyValue)
        {
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new ValidationContext(this) { MemberName = PropertyName };
            Validator.TryValidateProperty(PropertyValue, context, results);

            if (results.Any())
            {
                Errors[PropertyName] = results.Select(r => r.ErrorMessage).ToList();
            }
            else
            {
                Errors.Remove(PropertyName);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(PropertyName));
        }
        public MainViewModel()
        {
            //PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => true, (p) => Password = p.Password);
            SubmitCommand = new RelayCommand<object>((p) => { return Validator.TryValidateObject(this, new ValidationContext(this), null); }, (p) => { MessageBox.Show("Submitted"); });
        }
        private string _Name;
        [Required(ErrorMessage = "Name is not empty")]
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                Validate(nameof(Name), value);
            }
        }
        private string _Email;
        [Required(ErrorMessage = "Email is not empty")]
        [EmailAddress(ErrorMessage ="Email phải đúng định dạng")]
        public string Email
        {
            get { return _Email; }
            set
            {
                _Email = value;
                Validate(nameof(Email), value);
            }
        }
        //private string _Password;
        //[Required(ErrorMessage = "Password is not empty")]
        //[MinLength(6,ErrorMessage ="Password phải có ít nhất 6 ký tự")]
        //public string Password
        //{
        //    get { return _Password; }
        //    set
        //    {
        //        _Password = value;
        //        Validate(nameof(Password), value);
        //    }
        //}
        public ICommand SubmitCommand { get; set; }
        //public ICommand PasswordChangedCommand { get; set; }
    }
}
