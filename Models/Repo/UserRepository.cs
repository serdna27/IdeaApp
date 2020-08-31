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

        public RefreshToken SaveRefreshToken(User usr,RefreshToken token)
        {
            usr.Tokens.Add(token);
            this.Update(usr);
            return token;
        }
    }


}