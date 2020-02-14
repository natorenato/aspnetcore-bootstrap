using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domains.Queries
{
    public static class ContactQuery
    {
        public const string LIST_BY_USERID = @"
            SELECT ContactId, 
                   Email, 
                   UserId
            FROM Contato 
            WHERE UserId = @UserId;
        ";

        public const string GET = @"
            SELECT ContactId, 
                   Email, 
                   UserId
            FROM Contato 
            WHERE ContactId = @Id;
        ";

        public const string INSERT = @"
            INSERT INTO Contato 
                        (Email,
                        UserId)
                  VALUES (@Email,
                          @UserId);          
        ";

        public const string UPDATE = @"
            UPDATE Contato 
               SET Email = @Email
               WHERE ContactId = @Id;
        ";

        public const string DELETE = @"
            DELETE FROM Contato
                  WHERE ContactId = @Id;
        ";

        public const string TOTAL = @"
            SELECT COUNT(1)
              FROM Contato;
        ";

        public const string EXISTS_EMAIL = @"
            SELECT count(1) 
              FROM Contato
             WHERE Email = @Email;
        ";

        public const string EXISTS_SAME_EMAIL = @"
            SELECT count(1) 
              FROM Contato
             WHERE Email = @Email
               AND ContactId != @Id;
        ";
    }
}
