using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IdeaApp.Models.Repo.Base;
using Microsoft.EntityFrameworkCore;

namespace IdeaApp.Models.Repo
{
    public interface IUserRepository:IRepository<User>{
        RefreshToken SaveRefreshToken(User usr,RefreshToken token);
        RefreshToken GetRefreshToken(string refreshToken);

        void ExpireToken(RefreshToken token);

        User GetByUserName(string username);
    }
    public class UserRepository : MainRepository<User>,IUserRepository
    {
        public UserRepository(DbContext context):base(context)
        {
            
        }

        public void ExpireToken(RefreshToken token)
        {   
            token.Expiration=DateTime.UtcNow;
            Context.Set<RefreshToken>().Update(token);
            Context.SaveChanges();
        }

        public override User GetById(int id){

            return Context.Set<User>().Include(us=>us.Tokens)
            .Where(us=>us.Id==id).FirstOrDefault();
            

        }

        public User GetByUserName(string username)
        {
            return Context.Set<User>().Include(us => us.Tokens)
            .Where(k => k.UserName == username || k.Email == username).FirstOrDefault();
            
        }

        public RefreshToken GetRefreshToken(string refreshToken)
        {
            
            return Context.Set<RefreshToken>().Include(tk=>tk.User)
                .Where(tok=>tok.Token==refreshToken).FirstOrDefault();
            
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