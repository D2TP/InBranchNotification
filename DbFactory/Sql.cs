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

        public const string SelectADUserAndBranchName = "SELECT r.category_id, r.id as role_id, a.id, a.email,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,r.role_name ,b.branch_name  FROM inb_user_role AS u  INNER JOIN inb_aduser AS a  ON u.ad_user_id = a.id  INNER JOIN inb_role AS r ON r.id = u.role_id INNER JOIN inb_branch AS b ON a.branch_id = b.id where a.user_name=# and u.active=1 ";

        public const string SelectADUserAndBranch = "SELECT a.id, a.email,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,b.branch_name FROM  inb_aduser AS a  INNER JOIN inb_branch AS b ON a.branch_id = b.id";
        public const string SelectAllADUserAndRoleName = "SELECT a.id, a.email,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,r.role_name ,b.branch_name FROM inb_user_role AS u  INNER JOIN inb_aduser AS a  ON u.ad_user_id = a.id  INNER JOIN inb_role AS r ON r.id = u.role_id INNER JOIN inb_branch AS b ON a.branch_id = b.id";
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
   //     public const string UpdatePriviledge = "UPDATE inb_priviledge SET priviledge_name=#  WHERE id=#";
        public const string DeleteRolePriviledge = "DELETE FROM inb_role_priviledge WHERE  id=#";

        //Roles
        public const string SelectRoles = "SELECT id,category_id, role_name  FROM inb_role";
        public const string SelectOneRole = "SELECT id,category_id, role_name  FROM inb_role where id=#";
        public const string InsertRole = "INSERT INTO  inb_role  (id,category_id, role_name )  VALUES (#,#,#)";
        public const string UpdateRole = "UPDATE inb_role SET role_name=#  , category_id=# WHERE id=#";
        public const string DeletRole = "DELETE FROM inb_role WHERE  id=#";


        //Branches
        public const string SelectBranches = "SELECT id , branch_name , region_id   FROM inb_branch";
        public const string SelectOneBranch = "SELECT id , branch_name , region_id   FROM inb_branch where id=#";
        public const string InsertBranch = "INSERT INTO  inb_branch  (id , branch_name , region_id )  VALUES (#,#,#)";
        public const string UpdateBranch = "UPDATE inb_branch SET branch_name=#  , region_id=# WHERE id=#";
        public const string DeletBranch = "DELETE FROM inb_branch WHERE  id=#";

        //public const string SelectCustomerProfileByCustomerIdAndBvn = "SELECT id, password, bvn, nickname, first_name, last_name, other_name, email, mobile_no, status,failed_login, cif_id FROM cust_profile where cif_id = # ";
        //public const string SelectCustomerProfileByBvn = "SELECT id, password, bvn, nickname, first_name, last_name, other_name, email, mobile_no, status,failed_login, cif_id FROM cust_profile where bvn = #";
        //public const string SelectCustomerProfileByCustomerIdAndBvnDevice = "SELECT a.id, a.password, a.bvn, a.nickname, a.first_name, a.last_name, a.other_name, a.email, a.mobile_no, a.status,a.failed_login, a.cif_id, b.mac_address FROM cust_profile a left outer join (select mac_address, cif_id from app_cust_device where status=#) b on a.cif_id = b.cif_id where a.cif_id = # and a.bvn = #";
        //public const string SelectCustomerProfileByCustomerIdDevice = "SELECT a.id, a.password, a.bvn, a.nickname, a.first_name, a.last_name, a.other_name, a.email, a.mobile_no, a.status,a.failed_login, a.cif_id, b.mac_address FROM cust_profile a left outer join (select mac_address, cif_id from app_cust_device where status=#) b on a.cif_id = b.cif_id where a.cif_id = #";

        //public const string SelectCustomerProfileBvn = "SELECT date_of_birth FROM cust_profile where bvn = #";

        //public const string UpdateCustomerFailedLogin = "UPDATE cust_profile SET failed_login = # + failed_login WHERE cif_id=#";
        //public const string UpdateCustomerStatus = "UPDATE cust_profile SET status=# WHERE cif_id=#";
        //public const string InsertRefreshToken = "INSERT INTO cust_refresh_token (jti_id, cif_id, token, expired_on, created_on, created_ip, created_mac) VALUES(#, #, #, #, #, #, #)";
        //public const string SelectRefreshToken = "SELECT jti_id, cif_id, token, expired_on, created_on, created_ip, created_mac, revoked_on, revoked_ip, revoked_mac, status FROM cust_refresh_token where jti_id = #";
        //public const string UpdateRefreshToken = "UPDATE cust_refresh_token SET jti_id=# WHERE jti_id=# and cif_id = #";
        //public const string UpdateRevokeRefreshToken = "UPDATE cust_refresh_token SET revoked_on=#, revoked_ip=#, revoked_mac=#, status=# WHERE jti_id=# and cif_id = #";
        //public const string InsertCustomerProfile = "INSERT INTO cust_profile (id, cif_id, bvn, first_name, last_name, other_name, email, mobile_no, password, nickname, password_history) VALUES(#, #, #, #, #, #, #, #, #, #, #)";
        //public const string UpdateCustomerProfile = "UPDATE cust_profile SET first_name=#, last_name=#, other_name=#, email=#, mobile_no=# WHERE cif_id=#";
        //public const string UpdateCustomerPasswordPin = "UPDATE cust_profile SET pin=#, password=#, nickname=#, pin_history=#, password_history=#  WHERE cif_id=#";
        //public const string UpdateCustomerPin = "UPDATE cust_profile SET pin=#, pin_histor=# WHERE bvn=#";
        //public const string UpdateCustomerPinByCustomerIdAndBvn = "UPDATE cust_profile SET pin=#, pin_history=# WHERE cif_id=# and bvn=#";
        //public const string UpdateCustomerDeviceInactive = "update app_cust_device set status = #, date_deactivated =# where cif_id = # and status = #";
        //public const string InsertCustomerDevice = "INSERT INTO app_cust_device (id, mac_address, remark, cif_id, cust_profile_id) VALUES(#, #, #, #, #)";
        //public const string SelectMobileDeviceExists = "SELECT cif_id FROM app_cust_device WHERE cif_id=# and upper(mac_address)=# and status=#";

        //public const string InsertCustomerSecurityQuestion = "INSERT INTO cust_securty_questions (id, question, answer, cif_id) VALUES(#, #, #, #)";
        //public const string DeleteCustomerSecurityQuestion = "DELETE FROM cust_securty_questions WHERE cif_id=#";
        //public const string InsertNewCustomer = "INSERT INTO fbnquestDev.dbo.cust_new_lead (id, first_name, last_name, other_name, email, mobile_no) VALUES(#, #, #, #, #, #)";
    }
}