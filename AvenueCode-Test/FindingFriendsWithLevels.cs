using System;
using System.Collections.Generic;
using System.Linq;

namespace AvenueCodeTest
{
    public class Member
    {
        public string Email { get; private set; }

        public ICollection<Member> Friends { get; private set; }

        public Member(string email) : this(email, new List<Member>())
        {
        }

        public Member(string email, ICollection<Member> friends)
        {
            this.Email = email;
            this.Friends = friends;
        }

        public void AddFriends(ICollection<Member> friends)
        {
            foreach (Member friend in friends)
                this.Friends.Add(friend);
        }

        public void AddFriend(Member friend)
        {
            this.Friends.Add(friend);
        }
    }

    public class Friends
    {
        public static List<Member> GetFriendsOfDegree(Member member, int degree)
        {
            /*
                          A
                      B      C
                    D   E   B  F

            */
            ICollection<Member> friends = member.Friends;
            if (degree == 1) return friends.ToList();

            var frnds = GetImmidiateFriends( member, 0, degree - 1 );

            return frnds;

        }

        public static List<Member> GetImmidiateFriends(Member member, int degree, int max)
        {
            if (member.Friends == null) return new List<Member>();
            if (degree > max ) return new List<Member>();
            if (degree == max) return member.Friends.ToList();
            
            List<Member> _frnd = new List<Member>();

            foreach(var friend in member.Friends)
            {
                _frnd.AddRange( GetImmidiateFriends( friend, degree + 1, max ) );
            }
            degree++;

            return _frnd;
        }

        public static void Try()
        {
            Member a = new Member("A");
            Member b = new Member("B");
            Member c = new Member("C");

            a.AddFriend(b);
            b.AddFriend(c);

            foreach (Member friend in GetFriendsOfDegree(a, 2))
                Console.WriteLine(friend.Email);
        }
    }
    
}