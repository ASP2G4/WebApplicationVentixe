using WebApplicationVentixe.Models.Profile;
using WebApplicationVentixe.Models.SignUp;
using WebApplicationVentixe.Protos;

namespace WebApplicationVentixe.Mappers
{
    public static class ProfileMapper
    {
        public static CreateProfileRequest MapToCreateProfileRequest(this ProfileViewModel model, string userId)
        {
            return new CreateProfileRequest
            {
                Profile = new Profile
                {
                    UserId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = new ProfileAddress
                    {
                        StreetName = model.StreetName,
                        PostalCode = model.PostalCode,
                        City = model.City
                    }
                }
            };
        }

        public static CreateProfileRequest MapToCreateProfileRequest(this ProfileInformationViewModel model, string userId)
        {
            return new CreateProfileRequest
            {
                Profile = new Profile
                {
                    UserId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = new ProfileAddress
                    {
                        StreetName = model.StreetName,
                        PostalCode = model.PostalCode,
                        City = model.City
                    }
                }
            };
        }

        public static UpdateProfileRequest MapToUpdateProfileRequest(this ProfileViewModel model, string userId)
        {
            return new UpdateProfileRequest
            {
                Profile = new Profile
                {
                    UserId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = new ProfileAddress
                    {
                        StreetName = model.StreetName,
                        PostalCode = model.PostalCode,
                        City = model.City
                    }
                }
            };
        }

        public static ProfileViewModel MapToProfileViewModel(this Profile profile)
        {
            if (profile == null) return new ProfileViewModel();
            return new ProfileViewModel
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                PhoneNumber = profile.PhoneNumber,
                StreetName = profile.Address?.StreetName,
                PostalCode = profile.Address?.PostalCode,
                City = profile.Address?.City
            };
        }
    }
}
