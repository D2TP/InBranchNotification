namespace DbFactory
{
    public class Sql
    {

        public const string InsertADUser = "INSERT INTO  inb_aduser (user_name,first_name,last_name,active)VALUES(#, #,#,#)";
        public const string SelectADUser = "SELECT id, user_name, first_name, last_name], active, entry_date FROM inb_aduser where user_name =#";
        public const string SelectRole = "SELECT id,role_name,category_id  FROM inb_role";
        public const string SelectADUserAndRoleName = "SELECT a.id,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,r.role_name  FROM inb_user_role AS u  INNER JOIN inb_aduser AS a  ON u.ad_user_id = a.id  INNER JOIN inb_role AS r ON r.id = u.role_id where a.id=#";
        public const string SelectAllADUserAndRoleName = "SELECT a.id,a.active,a.entry_date,a.user_name,a.first_name,a.last_name ,r.role_name  FROM inb_user_role AS u  INNER JOIN inb_aduser AS a  ON u.ad_user_id = a.id  INNER JOIN inb_role AS r ON r.id = u.role_id";
        public const string InsertUserRole = "INSERT INTO  inb_user_role  (ad_user_id , role_id )  VALUES (#,#)";

        public const string SelectCustomerProfileByCustomerIdAndBvn = "SELECT id, password, bvn, nickname, first_name, last_name, other_name, email, mobile_no, status,failed_login, cif_id FROM cust_profile where cif_id = # and bvn = #";
        public const string SelectCustomerProfileByBvn = "SELECT id, password, bvn, nickname, first_name, last_name, other_name, email, mobile_no, status,failed_login, cif_id FROM cust_profile where bvn = #";
        public const string SelectCustomerProfileByCustomerIdAndBvnDevice = "SELECT a.id, a.password, a.bvn, a.nickname, a.first_name, a.last_name, a.other_name, a.email, a.mobile_no, a.status,a.failed_login, a.cif_id, b.mac_address FROM cust_profile a left outer join (select mac_address, cif_id from app_cust_device where status=#) b on a.cif_id = b.cif_id where a.cif_id = # and a.bvn = #";
        public const string SelectCustomerProfileByCustomerIdDevice = "SELECT a.id, a.password, a.bvn, a.nickname, a.first_name, a.last_name, a.other_name, a.email, a.mobile_no, a.status,a.failed_login, a.cif_id, b.mac_address FROM cust_profile a left outer join (select mac_address, cif_id from app_cust_device where status=#) b on a.cif_id = b.cif_id where a.cif_id = #";

        public const string SelectCustomerProfileBvn = "SELECT date_of_birth FROM cust_profile where bvn = #";

        public const string UpdateCustomerFailedLogin = "UPDATE cust_profile SET failed_login = # + failed_login WHERE cif_id=#";
        public const string UpdateCustomerStatus = "UPDATE cust_profile SET status=# WHERE cif_id=#";
        public const string InsertRefreshToken = "INSERT INTO cust_refresh_token (jti_id, cif_id, token, expired_on, created_on, created_ip, created_mac) VALUES(#, #, #, #, #, #, #)";
        public const string SelectRefreshToken = "SELECT jti_id, cif_id, token, expired_on, created_on, created_ip, created_mac, revoked_on, revoked_ip, revoked_mac, status FROM cust_refresh_token where jti_id = #";
        public const string UpdateRefreshToken = "UPDATE cust_refresh_token SET jti_id=# WHERE jti_id=# and cif_id = #";
        public const string UpdateRevokeRefreshToken = "UPDATE cust_refresh_token SET revoked_on=#, revoked_ip=#, revoked_mac=#, status=# WHERE jti_id=# and cif_id = #";
        public const string InsertCustomerProfile = "INSERT INTO cust_profile (id, cif_id, bvn, first_name, last_name, other_name, email, mobile_no, password, nickname, password_history) VALUES(#, #, #, #, #, #, #, #, #, #, #)";
        public const string UpdateCustomerProfile = "UPDATE cust_profile SET first_name=#, last_name=#, other_name=#, email=#, mobile_no=# WHERE cif_id=#";
        public const string UpdateCustomerPasswordPin = "UPDATE cust_profile SET pin=#, password=#, nickname=#, pin_history=#, password_history=#  WHERE cif_id=#";
        public const string UpdateCustomerPin = "UPDATE cust_profile SET pin=#, pin_histor=# WHERE bvn=#";
        public const string UpdateCustomerPinByCustomerIdAndBvn = "UPDATE cust_profile SET pin=#, pin_history=# WHERE cif_id=# and bvn=#";
        public const string UpdateCustomerDeviceInactive = "update app_cust_device set status = #, date_deactivated =# where cif_id = # and status = #";
        public const string InsertCustomerDevice = "INSERT INTO app_cust_device (id, mac_address, remark, cif_id, cust_profile_id) VALUES(#, #, #, #, #)";
        public const string SelectMobileDeviceExists = "SELECT cif_id FROM app_cust_device WHERE cif_id=# and upper(mac_address)=# and status=#";

        public const string InsertCustomerSecurityQuestion = "INSERT INTO cust_securty_questions (id, question, answer, cif_id) VALUES(#, #, #, #)";
        public const string DeleteCustomerSecurityQuestion = "DELETE FROM cust_securty_questions WHERE cif_id=#";
        public const string InsertNewCustomer = "INSERT INTO fbnquestDev.dbo.cust_new_lead (id, first_name, last_name, other_name, email, mobile_no) VALUES(#, #, #, #, #, #)";
    }
}