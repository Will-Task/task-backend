using Volo.Abp.Domain.Entities;

namespace Business.Models
{
    public class LocalizationText : Entity
    {
        public string LanguageCode { get; set; }

        public string Category { get; set; }

        public string ItemKey { get; set; }

        public string ItemValue { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { LanguageCode, Category, ItemKey};
        }
    }
}
