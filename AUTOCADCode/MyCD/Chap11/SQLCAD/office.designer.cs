﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.488
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SQLCAD
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="db_sample")]
	public partial class officeDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region 可扩展性方法定义
    partial void OnCreated();
    partial void InsertEmployee(Employee instance);
    partial void UpdateEmployee(Employee instance);
    partial void DeleteEmployee(Employee instance);
    partial void InsertRoom(Room instance);
    partial void UpdateRoom(Room instance);
    partial void DeleteRoom(Room instance);
    #endregion
		
		public officeDataContext() : 
				base(global::SQLCAD.Properties.Settings.Default.db_sampleConnectionString1, mappingSource)
		{
			OnCreated();
		}
		
		public officeDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public officeDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public officeDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public officeDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Employee> Employee
		{
			get
			{
				return this.GetTable<Employee>();
			}
		}
		
		public System.Data.Linq.Table<Room> Room
		{
			get
			{
				return this.GetTable<Room>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Employee")]
	public partial class Employee : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Last_Name;
		
		private string _First_Name;
		
		private string _Room_No;
		
		private EntityRef<Room> _Room;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnLast_NameChanging(string value);
    partial void OnLast_NameChanged();
    partial void OnFirst_NameChanging(string value);
    partial void OnFirst_NameChanged();
    partial void OnRoom_NoChanging(string value);
    partial void OnRoom_NoChanged();
    #endregion
		
		public Employee()
		{
			this._Room = default(EntityRef<Room>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Last_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Last_Name
		{
			get
			{
				return this._Last_Name;
			}
			set
			{
				if ((this._Last_Name != value))
				{
					this.OnLast_NameChanging(value);
					this.SendPropertyChanging();
					this._Last_Name = value;
					this.SendPropertyChanged("Last_Name");
					this.OnLast_NameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_First_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string First_Name
		{
			get
			{
				return this._First_Name;
			}
			set
			{
				if ((this._First_Name != value))
				{
					this.OnFirst_NameChanging(value);
					this.SendPropertyChanging();
					this._First_Name = value;
					this.SendPropertyChanged("First_Name");
					this.OnFirst_NameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Room_No", DbType="NVarChar(50)")]
		public string Room_No
		{
			get
			{
				return this._Room_No;
			}
			set
			{
				if ((this._Room_No != value))
				{
					if (this._Room.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnRoom_NoChanging(value);
					this.SendPropertyChanging();
					this._Room_No = value;
					this.SendPropertyChanged("Room_No");
					this.OnRoom_NoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Room_Employee", Storage="_Room", ThisKey="Room_No", OtherKey="Room_No", IsForeignKey=true)]
		public Room Room
		{
			get
			{
				return this._Room.Entity;
			}
			set
			{
				Room previousValue = this._Room.Entity;
				if (((previousValue != value) 
							|| (this._Room.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Room.Entity = null;
						previousValue.Employee.Remove(this);
					}
					this._Room.Entity = value;
					if ((value != null))
					{
						value.Employee.Add(this);
						this._Room_No = value.Room_No;
					}
					else
					{
						this._Room_No = default(string);
					}
					this.SendPropertyChanged("Room");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Room")]
	public partial class Room : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Room_No;
		
		private string _Room_Type;
		
		private string _Entity_Handle;
		
		private EntitySet<Employee> _Employee;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnRoom_NoChanging(string value);
    partial void OnRoom_NoChanged();
    partial void OnRoom_TypeChanging(string value);
    partial void OnRoom_TypeChanged();
    partial void OnEntity_HandleChanging(string value);
    partial void OnEntity_HandleChanged();
    #endregion
		
		public Room()
		{
			this._Employee = new EntitySet<Employee>(new Action<Employee>(this.attach_Employee), new Action<Employee>(this.detach_Employee));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Room_No", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string Room_No
		{
			get
			{
				return this._Room_No;
			}
			set
			{
				if ((this._Room_No != value))
				{
					this.OnRoom_NoChanging(value);
					this.SendPropertyChanging();
					this._Room_No = value;
					this.SendPropertyChanged("Room_No");
					this.OnRoom_NoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Room_Type", DbType="NVarChar(50)")]
		public string Room_Type
		{
			get
			{
				return this._Room_Type;
			}
			set
			{
				if ((this._Room_Type != value))
				{
					this.OnRoom_TypeChanging(value);
					this.SendPropertyChanging();
					this._Room_Type = value;
					this.SendPropertyChanged("Room_Type");
					this.OnRoom_TypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Entity_Handle", DbType="NVarChar(50)")]
		public string Entity_Handle
		{
			get
			{
				return this._Entity_Handle;
			}
			set
			{
				if ((this._Entity_Handle != value))
				{
					this.OnEntity_HandleChanging(value);
					this.SendPropertyChanging();
					this._Entity_Handle = value;
					this.SendPropertyChanged("Entity_Handle");
					this.OnEntity_HandleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Room_Employee", Storage="_Employee", ThisKey="Room_No", OtherKey="Room_No")]
		public EntitySet<Employee> Employee
		{
			get
			{
				return this._Employee;
			}
			set
			{
				this._Employee.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Employee(Employee entity)
		{
			this.SendPropertyChanging();
			entity.Room = this;
		}
		
		private void detach_Employee(Employee entity)
		{
			this.SendPropertyChanging();
			entity.Room = null;
		}
	}
}
#pragma warning restore 1591
