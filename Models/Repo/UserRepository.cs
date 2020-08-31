using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IdeaApp.Models.Repo.Base;
using Microsoft.EntityFrameworkCore;

namespace IdeaApp.Models.Repo
{
    public interface IUserRepository:IRepository<User>{

        RefreshToken SaveRefreshToken(User usr,RefreshToken token);
    }
    public class UserRepository : MainRepository<User>,IUserRepository
    {
        public UserRepository(DbContext context):base(context)
        {
            
        }

        
        public override User GetById(int id){

            var usr = Context.Set<User>().Find(id);
            
            Context.Entry(usr)
            .Collection(us=>us.Tokens).Load();

            return usr;

        }

        public RefreshToken SaveRefreshToken(User usr,RefreshToken token)
        {
            usr =this.GetById(usr.Id);
            
            usr.Tokens.Add(token);
            this.Update(usr);
            return token;
        }
    }


}