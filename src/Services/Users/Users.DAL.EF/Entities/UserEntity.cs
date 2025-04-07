using System.ComponentModel.DataAnnotations;

namespace Users.DAL.Entities {
    public class UserEntity {
        [Key]
        public Guid id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
    }
}