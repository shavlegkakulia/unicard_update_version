using System;
using Kunicardus.Core.UnicardApiProvider;
using Newtonsoft.Json;

namespace Kunicardus.Core.Models.DataTransferObjects
{
	public class RegisterUserRequest : UnicardApiBaseRequest
	{
		public RegisterUserRequest ()
		{
			MerritalStatus = "0";
			FamilyIncome = "0";
			FamilyMembers = "0";
			FamilyMembersU18 = "0";
			CarQuantity = "0";
			FamilyIncome = "0";
			Income = "0";
			Gender = "0";
			Street = "0";
			City = "0";
			Raion = "0";
			District = "0";
			Address = "0";
			FbId = "";
			WorkSphere = "0";
			UnicardNumber = "";
			WorkAddress = "0";
			WorkDescription = "0";
			HomePhone = "0";
			AdditionalEmail = "";
			SMSCode = "";

		}

		[JsonProperty ("user_name")]
		public string UserName { get; set; }

		[JsonProperty ("password")]
		public string Password { get; set; }

		[JsonProperty ("fb_token")]
		public string FbId { get; set; }

		[JsonProperty ("card")]
		public string UnicardNumber { get; set; }

		[JsonProperty ("new_card_registration")]
		public string NewCardRegistration { get; set; }

		[JsonProperty ("sms_code_otp")]
		public string SMSCode { get; set; }

		[JsonProperty ("person_code")]
		public string UserIdNumber { get; set; }

		[JsonProperty ("phone")]
		public string PhoneNumber { get; set; }

		[JsonProperty ("name")]
		public string Name { get; set; }

		[JsonProperty ("surname")]
		public string Surname { get; set; }

		[JsonProperty ("email")]
		public string Email { get; set; }

		[JsonProperty ("sex")]
		public string Gender { get; set; }

		[JsonIgnore]
		public DateTime? DateOfBirth { get; set; }

		[JsonProperty ("birth_date")]
		public string DateOfBorthForJSON {
			get { 
				if (DateOfBirth.HasValue)
					return DateOfBirth.Value.ToString (Constants.ApiDateTimeFormat);
				else
					return string.Empty;
			}
		}

		[JsonProperty ("home_phone")]
		public string HomePhone { get; set; }

		[JsonProperty ("add_email")]
		public string AdditionalEmail { get; set; }

		[JsonProperty ("city")]
		public string City { get; set; }

		[JsonProperty ("raion")]
		public string Raion { get; set; }

		[JsonProperty ("district")]
		public string District { get; set; }

		[JsonProperty ("street")]
		public string Street { get; set; }

		[JsonProperty ("address")]
		public string Address { get; set; }

		[JsonProperty ("working_sphere")]
		public string WorkSphere { get; set; }

		[JsonProperty ("work_desc")]
		public string WorkDescription { get; set; }

		[JsonProperty ("work_address")]
		public string WorkAddress { get; set; }

		[JsonProperty ("client_income")]
		public string Income { get; set; }

		[JsonProperty ("merrital_status")]
		public string MerritalStatus { get; set; }

		[JsonProperty ("fam_memb_quantity")]
		public string FamilyMembers { get; set; }

		[JsonProperty ("fam_member_under_18")]
		public string FamilyMembersU18 { get; set; }

		[JsonProperty ("car_quantity")]
		public string CarQuantity { get; set; }

		[JsonProperty ("family_income")]
		public string FamilyIncome { get; set; }

	}

}

