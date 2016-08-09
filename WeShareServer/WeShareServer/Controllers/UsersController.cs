using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace WeShareServer
{
    public class UsersController : ApiController
    {
        private WeShareEntities db = new WeShareEntities();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        private User GetUser(string email)
        {
            User user = db.Users.First(u => u.Email.ToLower() == email.ToLower());
            return user;
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user.Id == -1 && UserExists(user.Email))
            {
                User org = GetUser(user.Email);
                org.Name = user.Name;
                org.PictureUrl = user.PictureUrl;
                org.LinkdinUserProfileUrl = user.LinkdinUserProfileUrl;
            } else
                db.Users.Add(user);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }

        private bool UserExists(string email)
        {
            return db.Users.Count(e => e.Email.ToLower() == email.ToLower()) > 0;
        }
    }
}