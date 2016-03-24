using Newtonsoft.Json;
using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class UserListModel
    {
        public UserListModel(int page, int rows)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var us = from t in QC_DB.admin_user
                     orderby t.ID
                     select new UserModel
                       {
                           id = t.ID,
                           Username = t.Username,
                           Name = t.Name,
                           _GreatDate = t.GreatDate,
                           _LastDate = t.LastDate,
                           Type = t.user_type.Type
                       };

            this.total = QC_DB.admin_user.Count();

            this.rows = us.Skip(rows * (page - 1)).Take(rows).ToList();
        }

        public int total { get; set; }

        public List<UserModel> rows { get; set; }

        
    }

    public class UserModel
    {
       

        public int id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public DateTime? _GreatDate { get; set; }

        [JsonIgnore]
        public DateTime? _LastDate { get; set; }

        public string GreatDate
        {
            get
            {
                return _GreatDate == null ? null : _GreatDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string LastDate
        {
            get
            {
                return _LastDate == null ? null : _LastDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }



        public string Type { get; set; }
    }
}