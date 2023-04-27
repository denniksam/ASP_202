namespace ASP_202.Models.User
{
    public class ProfileModel
    {
        public Guid Id { get; set; }
        public String Login { get; set; }
        public String RealName { get; set; }
        public String Email { get; set; }
        public String? EmailCode { get; set; }
        public String? Avatar { get; set; }
        public DateTime RegisterDt { get; set; }
        public DateTime? LastEnterDt { get; set; }
        public Boolean IsEmailPublic { get; set; }
        public Boolean IsRealNamePublic { get; set; }
        public Boolean IsDatesPublic { get; set; }

        /// <summary>
        /// Чи профіль належить автентифікованому користувачу
        /// </summary>
        public Boolean IsPersonal { get; set; } = false;

        public ProfileModel(Data.Entity.User user)
        {
            // object mapping - відображення одного об'єкта на інший
            var userProps = user.GetType().GetProperties();  // Властивості, описані у типі об'єкта user
            var thisProps = this.GetType().GetProperties();
            foreach( var thisProp in thisProps )
            {
                // чи є у userProps такий саме thisProp
                var prop = userProps.FirstOrDefault(userProp =>
                    userProp.Name == thisProp.Name  // збіг за іменами властивостей
                    &&  // та тип userProp є приводимим до типу thisProp
                    userProp.PropertyType.IsAssignableTo( thisProp.PropertyType )
                );
                if( prop is not null )
                {
                    thisProp.SetValue(this, prop.GetValue(user));
                }
            }
        }
        public ProfileModel()
        {
            Login = RealName = Email = null!;
        }
    }
}
