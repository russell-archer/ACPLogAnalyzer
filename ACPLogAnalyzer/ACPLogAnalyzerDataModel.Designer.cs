﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]
#region EDM Relationship Metadata

[assembly: EdmRelationshipAttribute("ACPLogAnalyzerData", "ObservatoryObserver", "Observatory", System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(ACPLogAnalyzer.Observatory), "Observer", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(ACPLogAnalyzer.Observer), true)]
[assembly: EdmRelationshipAttribute("ACPLogAnalyzerData", "ObserverLog", "Observer", System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(ACPLogAnalyzer.Observer), "Log", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(ACPLogAnalyzer.Log), true)]

#endregion

namespace ACPLogAnalyzer
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class ACPLogAnalyzerDB : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new ACPLogAnalyzerDB object using the connection string found in the 'ACPLogAnalyzerDB' section of the application configuration file.
        /// </summary>
        public ACPLogAnalyzerDB() : base("name=ACPLogAnalyzerDB", "ACPLogAnalyzerDB")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new ACPLogAnalyzerDB object.
        /// </summary>
        public ACPLogAnalyzerDB(string connectionString) : base(connectionString, "ACPLogAnalyzerDB")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new ACPLogAnalyzerDB object.
        /// </summary>
        public ACPLogAnalyzerDB(EntityConnection connection) : base(connection, "ACPLogAnalyzerDB")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Observatory> Observatory
        {
            get
            {
                if ((_Observatory == null))
                {
                    _Observatory = base.CreateObjectSet<Observatory>("Observatory");
                }
                return _Observatory;
            }
        }
        private ObjectSet<Observatory> _Observatory;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Observer> Observer
        {
            get
            {
                if ((_Observer == null))
                {
                    _Observer = base.CreateObjectSet<Observer>("Observer");
                }
                return _Observer;
            }
        }
        private ObjectSet<Observer> _Observer;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Log> Log
        {
            get
            {
                if ((_Log == null))
                {
                    _Log = base.CreateObjectSet<Log>("Log");
                }
                return _Log;
            }
        }
        private ObjectSet<Log> _Log;

        #endregion
        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Observatory EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToObservatory(Observatory observatory)
        {
            base.AddObject("Observatory", observatory);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Observer EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToObserver(Observer observer)
        {
            base.AddObject("Observer", observer);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Log EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToLog(Log log)
        {
            base.AddObject("Log", log);
        }

        #endregion
    }
    

    #endregion
    
    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="ACPLogAnalyzerData", Name="Log")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Log : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Log object.
        /// </summary>
        /// <param name="logID">Initial value of the LogID property.</param>
        /// <param name="observerID">Initial value of the ObserverID property.</param>
        /// <param name="observatoryID">Initial value of the ObservatoryID property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        /// <param name="logStartDate">Initial value of the LogStartDate property.</param>
        /// <param name="logEndDate">Initial value of the LogEndDate property.</param>
        public static Log CreateLog(global::System.Int32 logID, global::System.Int32 observerID, global::System.Int32 observatoryID, global::System.String name, global::System.DateTime logStartDate, global::System.DateTime logEndDate)
        {
            Log log = new Log();
            log.LogID = logID;
            log.ObserverID = observerID;
            log.ObservatoryID = observatoryID;
            log.Name = name;
            log.LogStartDate = logStartDate;
            log.LogEndDate = logEndDate;
            return log;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 LogID
        {
            get
            {
                return _LogID;
            }
            set
            {
                if (_LogID != value)
                {
                    OnLogIDChanging(value);
                    ReportPropertyChanging("LogID");
                    _LogID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("LogID");
                    OnLogIDChanged();
                }
            }
        }
        private global::System.Int32 _LogID;
        partial void OnLogIDChanging(global::System.Int32 value);
        partial void OnLogIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 ObserverID
        {
            get
            {
                return _ObserverID;
            }
            set
            {
                OnObserverIDChanging(value);
                ReportPropertyChanging("ObserverID");
                _ObserverID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ObserverID");
                OnObserverIDChanged();
            }
        }
        private global::System.Int32 _ObserverID;
        partial void OnObserverIDChanging(global::System.Int32 value);
        partial void OnObserverIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 ObservatoryID
        {
            get
            {
                return _ObservatoryID;
            }
            set
            {
                OnObservatoryIDChanging(value);
                ReportPropertyChanging("ObservatoryID");
                _ObservatoryID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ObservatoryID");
                OnObservatoryIDChanged();
            }
        }
        private global::System.Int32 _ObservatoryID;
        partial void OnObservatoryIDChanging(global::System.Int32 value);
        partial void OnObservatoryIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.DateTime LogStartDate
        {
            get
            {
                return _LogStartDate;
            }
            set
            {
                OnLogStartDateChanging(value);
                ReportPropertyChanging("LogStartDate");
                _LogStartDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LogStartDate");
                OnLogStartDateChanged();
            }
        }
        private global::System.DateTime _LogStartDate;
        partial void OnLogStartDateChanging(global::System.DateTime value);
        partial void OnLogStartDateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.DateTime LogEndDate
        {
            get
            {
                return _LogEndDate;
            }
            set
            {
                OnLogEndDateChanging(value);
                ReportPropertyChanging("LogEndDate");
                _LogEndDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LogEndDate");
                OnLogEndDateChanged();
            }
        }
        private global::System.DateTime _LogEndDate;
        partial void OnLogEndDateChanging(global::System.DateTime value);
        partial void OnLogEndDateChanged();

        #endregion
    
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="ACPLogAnalyzerData", Name="Observatory")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Observatory : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Observatory object.
        /// </summary>
        /// <param name="observatoryID">Initial value of the ObservatoryID property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        /// <param name="latitude">Initial value of the Latitude property.</param>
        /// <param name="longitude">Initial value of the Longitude property.</param>
        public static Observatory CreateObservatory(global::System.Int32 observatoryID, global::System.String name, global::System.String latitude, global::System.String longitude)
        {
            Observatory observatory = new Observatory();
            observatory.ObservatoryID = observatoryID;
            observatory.Name = name;
            observatory.Latitude = latitude;
            observatory.Longitude = longitude;
            return observatory;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 ObservatoryID
        {
            get
            {
                return _ObservatoryID;
            }
            set
            {
                if (_ObservatoryID != value)
                {
                    OnObservatoryIDChanging(value);
                    ReportPropertyChanging("ObservatoryID");
                    _ObservatoryID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("ObservatoryID");
                    OnObservatoryIDChanged();
                }
            }
        }
        private global::System.Int32 _ObservatoryID;
        partial void OnObservatoryIDChanging(global::System.Int32 value);
        partial void OnObservatoryIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Latitude
        {
            get
            {
                return _Latitude;
            }
            set
            {
                OnLatitudeChanging(value);
                ReportPropertyChanging("Latitude");
                _Latitude = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Latitude");
                OnLatitudeChanged();
            }
        }
        private global::System.String _Latitude;
        partial void OnLatitudeChanging(global::System.String value);
        partial void OnLatitudeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Longitude
        {
            get
            {
                return _Longitude;
            }
            set
            {
                OnLongitudeChanging(value);
                ReportPropertyChanging("Longitude");
                _Longitude = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Longitude");
                OnLongitudeChanged();
            }
        }
        private global::System.String _Longitude;
        partial void OnLongitudeChanging(global::System.String value);
        partial void OnLongitudeChanged();

        #endregion
    
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="ACPLogAnalyzerData", Name="Observer")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Observer : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Observer object.
        /// </summary>
        /// <param name="observerID">Initial value of the ObserverID property.</param>
        /// <param name="observatoryID">Initial value of the ObservatoryID property.</param>
        /// <param name="surname">Initial value of the Surname property.</param>
        /// <param name="forename">Initial value of the Forename property.</param>
        public static Observer CreateObserver(global::System.Int32 observerID, global::System.Int32 observatoryID, global::System.String surname, global::System.String forename)
        {
            Observer observer = new Observer();
            observer.ObserverID = observerID;
            observer.ObservatoryID = observatoryID;
            observer.Surname = surname;
            observer.Forename = forename;
            return observer;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 ObserverID
        {
            get
            {
                return _ObserverID;
            }
            set
            {
                if (_ObserverID != value)
                {
                    OnObserverIDChanging(value);
                    ReportPropertyChanging("ObserverID");
                    _ObserverID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("ObserverID");
                    OnObserverIDChanged();
                }
            }
        }
        private global::System.Int32 _ObserverID;
        partial void OnObserverIDChanging(global::System.Int32 value);
        partial void OnObserverIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 ObservatoryID
        {
            get
            {
                return _ObservatoryID;
            }
            set
            {
                OnObservatoryIDChanging(value);
                ReportPropertyChanging("ObservatoryID");
                _ObservatoryID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ObservatoryID");
                OnObservatoryIDChanged();
            }
        }
        private global::System.Int32 _ObservatoryID;
        partial void OnObservatoryIDChanging(global::System.Int32 value);
        partial void OnObservatoryIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Surname
        {
            get
            {
                return _Surname;
            }
            set
            {
                OnSurnameChanging(value);
                ReportPropertyChanging("Surname");
                _Surname = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Surname");
                OnSurnameChanged();
            }
        }
        private global::System.String _Surname;
        partial void OnSurnameChanging(global::System.String value);
        partial void OnSurnameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Forename
        {
            get
            {
                return _Forename;
            }
            set
            {
                OnForenameChanging(value);
                ReportPropertyChanging("Forename");
                _Forename = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Forename");
                OnForenameChanged();
            }
        }
        private global::System.String _Forename;
        partial void OnForenameChanging(global::System.String value);
        partial void OnForenameChanged();

        #endregion
    
    }

    #endregion
    
}
