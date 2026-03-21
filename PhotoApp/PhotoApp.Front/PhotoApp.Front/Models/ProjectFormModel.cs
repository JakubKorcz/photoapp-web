using PhotoApp.Common.EnumShared;
using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Front.Models
{
    public class ProjectFormModel
    {
        [Required(ErrorMessage = "Nazwa projektu jest wymagana")]
        public string ProjectName { get; set; } = "";
        public DateOnly? PhotoShootDate { get; set; }
        public bool IsPernament { get; set; } = false;
        private ExpiryDateSelectOption _selectedExpiryDate = ExpiryDateSelectOption.TwoWeeks;
        public ExpiryDateSelectOption SelectedExpiryDate
        {
            get => _selectedExpiryDate;
            set
            {
                _selectedExpiryDate = value;

                // .Date kasuję aktulaną godzine i usatwia ją na 00:00
                ExpiryDate = value switch
                {
                    ExpiryDateSelectOption.TwoWeeks => DateTime.Now.AddDays(14).Date
                                     .AddHours(23).AddMinutes(59),
                    ExpiryDateSelectOption.OneMonth => DateTime.Now.AddMonths(1).Date
                                     .AddHours(23).AddMinutes(59),
                    ExpiryDateSelectOption.TwoMonths => DateTime.Now.AddMonths(2).Date
                                     .AddHours(23).AddMinutes(59),
                    _ => DateTime.Now.AddDays(14).Date.AddHours(23).AddMinutes(59),
                };
            }
        }

        public DateTime? ExpiryDate { get; set; } = DateTime.Now.AddDays(14).Date.AddHours(23).AddMinutes(59);
        public Language Language { get; set; } = Language.PL;
        [StringLength(4)]
        public string? Password { get; set; }
    }
}
