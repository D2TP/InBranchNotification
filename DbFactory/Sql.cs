namespace DbFactory
{
    public class Sql
    {
                                              
        //ADUSer and Role
        public const string InsertADUser = "INSERT INTO  inb_aduser (id,user_name,first_name,last_name,active,email,branch_id,created_by)VALUES(#,#,#,#,#,#,#,#)";
        public const string SelectADUser = "SELECT id,  email, user_name, first_name, last_name , active, entry_date FROM inb_aduser where user_name =#";
        public const string SelectADUserById = "SELECT id,  email, user_name, first_name, last_name , active, entry_date FROM inb_aduser where id =#";
        public const string SelectRole = "SELECT id,role_name,category_id  FROM inb_role where id=#";
        public const string SelectADUserAndRoleName = "SELECT r.category_id,   u.active as is_role_active , r.id as role_id, a.id, a.email,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,r.role_name ,b.branch_name  FROM inb_user_role AS u  INNER JOIN inb_aduser AS a  ON u.ad_user_id = a.id  INNER JOIN inb_role AS r ON r.id = u.role_id INNER JOIN inb_branch AS b ON a.branch_id = b.id where a.id=#";
        public const string SelectADUserByUserName = "SELECT r.category_id,   u.active as is_role_active , r.id as role_id, a.id, a.email,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,r.role_name ,b.branch_name  FROM inb_user_role AS u  INNER JOIN inb_aduser AS a  ON u.ad_user_id = a.id  INNER JOIN inb_role AS r ON r.id = u.role_id INNER JOIN inb_branch AS b ON a.branch_id = b.id where a.user_name=#";
        public const string SelectADUserList = "SELECT user_name, first_name ,last_name,email,active,entry_date,branch_id,modified_date,modified_by,created_by  FROM  inb_aduser ";

       
        public const string SelectADUserAndBranchName = "SELECT r.category_id, r.id as role_id, a.id, a.email,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,r.role_name ,b.branch_name  FROM inb_user_role AS u  INNER JOIN inb_aduser AS a  ON u.ad_user_id = a.id  INNER JOIN inb_role AS r ON r.id = u.role_id INNER JOIN inb_branch AS b ON a.branch_id = b.id where a.user_name=# and u.active=1 ";
        public const string SelectADUserCount = "SELECT count(*) as Total from  inb_aduser ";
        public const string SelectADUserActiveCount = "SELECT count(*) as  ActiveUser  from  inb_aduser where active=1 "; 
        public const string SelectADUserInactiveCount = "SELECT count(*) as  Inactive  from  inb_aduser where active=0 "; 
        public const string SelectADUserAndBranch = "SELECT a.id, a.email,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,b.branch_name FROM  inb_aduser AS a  INNER JOIN inb_branch AS b ON a.branch_id = b.id";
        public const string SelectAllADUserAndRoleName = "SELECT a.id, a.email,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,r.role_name ,b.branch_name FROM inb_user_role AS u  INNER JOIN inb_aduser AS a  ON u.ad_user_id = a.id  INNER JOIN inb_role AS r ON r.id = u.role_id INNER JOIN inb_branch AS b ON a.branch_id = b.id";
        public const string SelectAllADUserAndRoleNameBranch = "SELECT a.id, a.email as Email,a.active as Active,a.entry_date as EntryDate,a.user_name as UserName,a.first_name as FirstName,a.last_name as LastNmae,a.first_name+' '+a.last_name as DisplayName,  r.role_name as RoleName ,u.role_id as RoleId,  a.branch_id as BranchId,b.branch_name as BranchName FROM inb_user_role AS u INNER JOIN inb_aduser AS a ON u.ad_user_id = a.id INNER JOIN inb_role AS r ON r.id = u.role_id   INNER JOIN inb_branch AS b ON a.branch_id = b.id";
        public const string InsertUserRole = "INSERT INTO  inb_user_role  (id, ad_user_id , role_id, active )  VALUES (#,#,#,1)";
        public const string SelectUserBaseOnAdUserId = "Select  id, ad_user_id , role_id from inb_user_role where ad_user_id=#";

        public const string UpdateADUser = "UPDATE inb_aduser SET user_name=#, first_name=#,last_name=#, active=#, email=#,branch_id=#,modified_by=#,modified_date=# WHERE id=# ";
        public const string UpdatetUserRole = "UPDATE inb_user_role SET role_id=#, active=1 WHERE ad_user_id=# and role_id = #";
        public const string GetUserRole = "SELECT * from  inb_user_role   WHERE ad_user_id=# and role_id = #";

        public const string CheckIdAdUserIsActive = "SELECT count(*) from inb_aduser   WHERE active=# and user_name=#  ";
        public const string ActivateDeactivateADUser = "Update inb_aduser set active=#   WHERE  user_name=#";
       // public const string ActivateADUser = "Update inb_aduser set active=#   WHERE  id=#";
        public const string DeleteADUser = "DELETE FROM inb_aduser WHERE  id=#";
        public const string DeleteUserRoles = "DELETE FROM inb_user_role WHERE  ad_user_id=#";
        public const string DeleteSingleUserRoles = "DELETE FROM inb_user_role WHERE  ad_user_id=# and role_id=#";
        public const string ActivateDeactivateSingleUserRoles = "Update inb_user_role set active=# WHERE  ad_user_id=# and role_id=#";
        public const string CheckIdAdUserRoleIsActive = "SELECT count(*) from inb_user_role   WHERE  active=# and ad_user_id=# and role_id=#  ";

        //categories
        public const string SelectCatigories = "SELECT id,category_name  FROM inb_category";
        public const string SelectOneCatigory = "SELECT id,category_name  FROM inb_category where id=#";
        public const string InsertCatigory = "INSERT INTO  inb_category  (id,category_name )  VALUES (#,#)";
        public const string UpdateCatigory = "UPDATE inb_category SET category_name=#  WHERE id=#";
        public const string DeletCategory = "DELETE FROM inb_category WHERE  id=#";

        //region
        public const string SelectRegion = "SELECT id,region_name  FROM inb_region";
        public const string SelectOneRegion = "SELECT id,region_name  FROM inb_region where id=#";
        public const string InsertRegion = "INSERT INTO  inb_region  (id,region_name )  VALUES (#,#)";
        public const string UpdateRegion = "UPDATE inb_region SET region_name=#  WHERE id=#";
        public const string DeletRegion = "DELETE FROM inb_region WHERE  id=#";


        //permission
        public const string SelectPermission = "SELECT id,permission_name  FROM inb_permission";
        public const string SelectOnePermission = "SELECT id,permission_name  FROM inb_permission where id=#";
        public const string InsertPermission = "INSERT INTO  inb_permission  (id,permission_name )  VALUES (#,#)";
        public const string UpdatePermission = "UPDATE inb_permission SET permission_name=#  WHERE id=#";
        public const string DeletPermission = "DELETE FROM inb_permission WHERE  id=#";

        //priviledge
        public const string SelectPriviledge = "SELECT id,priviledge_name  FROM inb_priviledge";
        public const string SelectOnePriviledge = "SELECT id,priviledge_name  FROM inb_priviledge where id=#";
        public const string InsertPriviledge = "INSERT INTO  inb_priviledge  (id,priviledge_name )  VALUES (#,#)";
        public const string UpdatePriviledge = "UPDATE inb_priviledge SET priviledge_name=#  WHERE id=#";
        public const string DeletPriviledge = "DELETE FROM inb_priviledge WHERE  id=#";


        //RolePriviledge
        public const string SelectRolePriviledge = "SELECT    rp.id, rp.priviledge_id,pr.priviledge_name ,  rp.role_id,r.role_name FROM     dbo.inb_role_priviledge   as rp   INNER JOIN dbo.inb_priviledge as pr ON rp.priviledge_id = pr.id inner join inb_role as r on r.id=rp.role_id";

        public const string CheckIfRolePriviledgeExists = "SELECT count(*) from dbo.inb_role_priviledge where priviledge_id=#  and  role_id=#";
        public const string SelectOneRolePriviledge = "  SELECT    rp.id, rp.priviledge_id,pr.priviledge_name ,  rp.role_id,r.role_name FROM     dbo.inb_role_priviledge   as rp     INNER JOIN dbo.inb_priviledge as pr ON rp.priviledge_id = pr.id inner join inb_role as r on r.id=rp.role_id  where  r.id=#";
        public const string InsertRolePriviledge = "INSERT INTO  inb_role_priviledge (id,priviledge_id,role_id)  VALUES (#,#,#)";

        public const string RoleBasedPriviledge = "SELECT  inb_role_priviledge.id, inb_role_priviledge.priviledge_id, inb_role_priviledge.role_id, inb_role.role_name, inb_priviledge.priviledge_name FROM inb_role_priviledge INNER JOIN inb_priviledge ON inb_role_priviledge.priviledge_id = inb_priviledge.id INNER JOIN  inb_role ON inb_role_priviledge.role_id = inb_role.id";
        public const string DeleteRolePriviledge = "DELETE FROM inb_role_priviledge WHERE  id=#";

        //Roles
        public const string SelectRoles = "SELECT id,category_id, role_name  FROM inb_role";
        public const string SelectOneRole = "SELECT id,category_id, role_name  FROM inb_role where id=#";
        public const string InsertRole = "INSERT INTO  inb_role  (id,category_id, role_name )  VALUES (#,#,#)";
        public const string UpdateRole = "UPDATE inb_role SET role_name=#  , category_id=# WHERE id=#";
        public const string DeletRole = "DELETE FROM inb_role WHERE  id=#";


        //Notification
        public const string SelectNotifications = "SELECT id,title,type,notification_date,sender,body,completed   FROM inb_notification";
        public const string SelectOneNotification = "SELECT id,title,type,notification_date,sender,body,completed   FROM inb_notification where id=#";
        public const string InsertNotification = "INSERT INTO inb_notification (id,title,type,notification_date,sender,body,completed) VALUES ( #,#,#,#,#,#,#)";
        public const string StoredProcSearcNotification = "EXECUTE inb_sp_search_notification @id=#,@title=#,@type=#,@from_entry_date=#,@to_entry_date=#,@sender=#,@body=#,@completed=# ";

        public const string DeletBranch = "DELETE FROM inb_branch WHERE  id=#";

            }
}