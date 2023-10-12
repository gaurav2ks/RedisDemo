using System.ComponentModel.DataAnnotations;

namespace RedisDemo.Models
{
    [Serializable]
    public class UserModel
    {
        [Key]
        public int BeneficiaryId { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Mobile { get; set; }

        public string? Address { get; set; }
    }
}
