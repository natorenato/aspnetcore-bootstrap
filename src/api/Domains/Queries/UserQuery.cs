using System.Runtime.Serialization;

namespace API.Domains.Queries
{
    public static class UserQuery
    {
        public const string GET = @"
            SELECT UserId,
                   IdProfile as Profile,
                   IdCountry as Country,
                   CreatedBy,
                   Name,
                   Document,
                   Birthdate,
                   Active
              FROM Usuario
             WHERE UserId = @Id
               AND CreatedBy = @CreatedBy;
        ";

        public const string PAGINATE = @"
            SELECT
                   u.UserId,
                   u.IdProfile as Profile,
                   u.IdCountry as Country,
                   u.CreatedBy,
                   u.Name,
                   u.Document,
                   u.Birthdate,
                   u.Active
              FROM Usuario u
              ORDER BY u.UserId ASC
              OFFSET(@Offset) ROWS FETCH NEXT (@Limit) ROWS ONLY;
        ";

        public const string PAGINATE_LEFT_JOIN = @"
            SELECT
                   u.UserId,
                   u.IdProfile as Profile,
                   u.IdCountry as Country,
                   u.CreatedBy,
                   u.Name,
                   u.Document,
                   u.Birthdate,
                   u.Active,
                   c.ContactId,
                   c.Email,
                   c.UserId
              FROM Usuario u
              LEFT JOIN Contato c ON u.UserId = c.UserId 
              ORDER BY u.UserId ASC
              OFFSET(@Offset) ROWS FETCH NEXT (@Limit) ROWS ONLY;
        ";

        public const string TOTAL = @"
            SELECT COUNT(1)
              FROM Usuario
             WHERE CreatedBy = @CreatedBy;
        ";

        public const string INSERT = @"
            INSERT INTO Usuario 
                       (IdProfile,
                        IdCountry,
                        CreatedBy,
                        Name,
                        Document,
                        Birthdate,
                        Active)
                VALUES (@IdProfile,
                        @IdCountry,
                        @CreatedBy,
                        @Name,
                        @Document,
                        @Birthdate,
                        @Active);

            SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];               
        ";
        
        public const string UPDATE = @"
            UPDATE Usuario 
               SET IdProfile = @IdProfile,
                   IdCountry = @IdCountry,
                   Name = @Name,
                   Birthdate = @Birthdate,
                   Active = @Active
             WHERE UserId = @Id;
        ";

        public const string DELETE = @"
            DELETE FROM Usuario
                  WHERE UserId = @Id
                    AND CreatedBy = @CreatedBy;
        ";

        public const string ACTIVATE_DEACTIVATE = @"
            UPDATE Usuario 
               SET Active = @Active
             WHERE UserId = @Id
               AND CreatedBy = @CreatedBy;
        ";

        public const string EXISTS_DOCUMENT = @"
            SELECT count(1) 
              FROM Usuario
             WHERE Document = @Document;
        ";
    }
}
