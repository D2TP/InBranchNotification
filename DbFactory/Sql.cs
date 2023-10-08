using InBranchNotification.DTOs;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public const string SelectToken = "SELECT id, user_name, token, expires_on, created_on, revoked_on,active, refreshToken FROM inb_refresh_token where token = #";

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

        //Notification Types
        public const string SelectnotificationTypes = "SELECT id,notification_type  FROM inb_notification_type";
        public const string SelectOneotificationType = "SELECT id,notification_type  FROM inb_notification_type where id=#";
        public const string InsertNotificationTypes = "INSERT INTO  inb_notification_type  (id,notification_type )  VALUES (#,#)";
        public const string UpdateNotificationTypes = "UPDATE inb_notification_type SET notification_type=#  WHERE id=#";
        public const string DeletNotificationTypes = "DELETE FROM inb_notification_type WHERE  id=#";

        //Service Request Types
        public const string SelectServiceRequestTypes = "SELECT id,request_type  FROM inb_service_request_type";
        public const string SelectOneServiceRequestType = "SELECT id,request_type  FROM inb_service_request_type where id=#";
        public const string InsertServiceRequestTypes = "INSERT INTO  inb_service_request_type  (id,request_type )  VALUES (#,#)";
        public const string UpdateServiceRequestTypes = "UPDATE inb_service_request_type SET request_type=#  WHERE id=#";
        public const string DeletServiceRequestTypes = "DELETE FROM inb_service_request_type WHERE  id=#";

        //Service Request Status
        public const string SelectServiceRequestStatus = "SELECT id,status  FROM inb_service_request_status";
        public const string SelectOneServiceRequestStatus = "SELECT id,status  FROM inb_service_request_status where id=#";
        public const string InsertServiceRequestStatus = "INSERT INTO  inb_service_request_status  (id,status )  VALUES (#,#)";
        public const string UpdateServiceRequestStatus = "UPDATE inb_service_request_status SET status=#  WHERE id=#";
        public const string DeletServiceRequestStatuss = "DELETE FROM inb_service_request_status WHERE  id=#";

        //Service Request
        public const string SelectServiceRequest = "SELECT   inb_service_request.id, inb_service_request.service_request_type_id, " +
            "inb_service_request.service_request_status_id, inb_service_request.client, inb_service_request.cif_id," +
            " inb_service_request.service_request_date, inb_service_request.approver, inb_service_request.reviewer, inb_service_request.review_date, inb_service_request.approval_date," +
            "      inb_service_request_status.status, inb_service_request_type.request_type,inb_service_request.other_request_details" +
            " FROM     inb_service_request INNER JOIN  " +
            "      inb_service_request_type ON inb_service_request.service_request_type_id = inb_service_request_type.id INNER JOIN  " +
            "                inb_service_request_status ON inb_service_request.service_request_status_id = inb_service_request_status.id";
        
        public const string SelectOneServiceRequest = SelectServiceRequest+ "  where inb_service_request.id=#";
        public const string InsertServiceRequest = "INSERT INTO  inb_service_request  (id ,service_request_type_id,service_request_status_id,client,cif_id,other_request_details,service_request_date  )  VALUES (#,#,#,#,#,#,#)";
        public const string UpdateServiceRequests = "UPDATE inb_service_request SET service_request_type_id=#,service_request_status_id=#    WHERE id=#";
        public const string ReviewServiceRequests = "UPDATE inb_service_request SET reviewer=#,review_date=#  ,service_request_status_id=#  WHERE id=#";
        public const string ApproveServiceRequests = "UPDATE inb_service_request SET approver=#,approval_date=#  ,service_request_status_id=#  WHERE id=#";
        public const string DeletServiceRequest = "DELETE FROM inb_service_request WHERE  id=#";

        public const string StoredProcSearcServiceRequest = "EXECUTE inb_sp_search_service_request " +
            "@id=#,@service_request_status_id=#,@service_request_type_id=#," +
            "@client=#,@cif_id=#,@approver=#,@reviewer=#,@fromdate=#,@todate=# ";
        public const string ApproveServiceRequest = "UPDATE inb_service_request SET service_request_status_id=# ,approver=#,approval_date=# WHERE id=#";
        public const string ReviewServiceRequest = "UPDATE inb_service_request SET service_request_status_id=# ,reviewer=#,review_date=# WHERE id=#";
        public const string SelectRequest = " select * from inb_service_request where inb_service_request.id=#";

        public const string StoredProcSearcServiceHistoryRequest = "EXECUTE inb_sp_search_request_history " +
    "@id=#,@actor=#,@status=#,@from_entry_date=#,@to_entry_date=#,@service_request_id=#,@comment=#,@activity=# ";

        public const string SelectNotifications = "SELECT id,title,type,notification_date,sender,body,completed   FROM inb_notification";
        public const string SelectOneNotification = "SELECT id,title,type,notification_date,sender,body,completed,approved,approver,recipents,recipent_count    FROM inb_notification where id=#";
        public const string InsertNotification = "INSERT INTO inb_notification (id,title,type,notification_date,sender,body,completed,recipents,recipent_count,approved) VALUES ( #,#,#,#,#,#,#,#,#,#)";
        public const string StoredProcSearcNotification = "EXECUTE inb_sp_search_notification @id=#,@title=#,@type=#,@from_entry_date=#,@to_entry_date=#,@sender=#,@body=#,@completed=# ";
        public const string ApproveNotification = "UPDATE inb_notification SET approved=# ,approver=#,completed=# WHERE id=#";
        public const string DeletBranch = "DELETE FROM inb_branch WHERE  id=#";

        //Service Request History
  
        public const string InsertServiceRequestHistory = "INSERT INTO  inb_service_request_history  (id ,actor,activity,activity_date,comment,service_request_id,status,other_request_details  )  VALUES (#,#,#,#,#,#,#,#)";

    }
}