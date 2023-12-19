namespace PotatoService{
    public class Course{
        #region Properties

        public string? Title { get; set; }
        public string? MasterName {get ; set ;}
        public string? StartDate {get ; set ;}
        public bool? IsOnline {get ; set ;}
        public bool? IsInPerson {get ; set ;}
        public string? ShortLink{get ; set; }
        public string? HowLongIsCourse{get ; set;}
        public string? Sections{get ; set;}
        public string? FinalMessage{get ; set ;}

        #endregion
    }
}