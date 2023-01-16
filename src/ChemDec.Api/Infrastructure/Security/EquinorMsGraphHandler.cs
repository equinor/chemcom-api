using ChemDec.Api.GraphApi;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure.Security
{
    public class EquinorMsGraphHandler
    {
        readonly IGraphServiceProvider _graphServiceProvider;
        public EquinorMsGraphHandler(IGraphServiceProvider graphServiceProvider)
        {
            _graphServiceProvider = graphServiceProvider;
        }

        public async Task<User> GetUserAsync(string searchParam, string searchField = null)
        {

            switch (searchParam)
            {
                case "BOUVET\\stian.edvardsen":
                    searchParam = "STGA@STATOIL.COM";
                    break;

                case "BOUVET\\steffen.martinsen":
                    searchParam = "SMARTI@STATOIL.COM";
                    break;

                case "BOUVET\\jostein.soiland":
                    searchParam = "JSOI@STATOIL.COM";
                    break;
                case "DESKTOP-67U1GEE\\nebus":
                    searchParam = "RONNAV@STATOIL.COM";
                    break;
            }

            if (string.IsNullOrEmpty(searchField))
                searchField = "userprincipalname";
            if (searchParam.Contains("live.com#"))
                searchParam = searchParam.Substring(searchParam.IndexOf("#") + 1);
            string filter = $"{searchField} eq '{searchParam}'";
            var list = await GetUsersContainsAsync(searchParam);
            return list.FirstOrDefault();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            string filter = $"id eq '{id}'";
            var list = await UserLookUp(filter);
            return list.FirstOrDefault();
        }

        public async Task<IList<User>> GetUsersContainsAsync(string searchString, int? defaultTop = null)
        {
            string filter = $"startswith(userprincipalname, '{searchString}') or startswith(displayName, '{searchString}') or mail eq('{searchString}')";
            return await UserLookUp(filter, defaultTop);
        }


        public async Task<IList<Group>> GetGroupsStartsWithAsync(string searchString, int? defaultTop = null)
        {
            string filter = $"startswith(displayName, '{searchString}')";
            return await GroupLookUp(filter, defaultTop);
        }

        public async Task<IList<Group>> GetGroupsMultipleStartsWithAsync(string searchString1, string searchString2, int? defaultTop = null)
        {
            string filter = $"startswith(displayName, '{searchString1}') or startswith(displayName, '{searchString2}')";
            return await GroupLookUp(filter, defaultTop);
        }

        private async Task<IGraphServiceUsersCollectionPage> UserLookUp(string filter, int? defaultTop = null)
        {
            var properties = "DisplayName,GivenName,Id,JobTitle,Mail,MobilePhone,OfficeLocation,Surename,UserPrincipalName,Department, UserType, CompanyName, MailNickName";
            var client = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All", "GroupMember.Read.All" });
            if (defaultTop.HasValue)
                return await client.Users.Request().Filter(filter).Select(properties).Top(defaultTop.Value).GetAsync();

            return await client.Users.Request().Filter(filter).Select(properties).GetAsync();
        }



        private async Task<IGraphServiceGroupsCollectionPage> GroupLookUp(string filter, int? defaultTop = null)
        {
            var client = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All", "GroupMember.Read.All" });

            if (defaultTop.HasValue)
                return await client.Groups.Request().Filter(filter).Top(defaultTop.Value).GetAsync();
            return await client.Groups.Request().Filter(filter).GetAsync();
        }

        public async Task<IList<Group>> GetGroupMembershipForUser(string userId, string filter, int? defaultTop = null)
        {
            var client = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All", "GroupMember.Read.All" });

            List<Group> filteredList = new List<Group>();
            var currentUser = client.Users[userId];

            var groupPage = await currentUser.MemberOf.Request().GetAsync();
            var groups = groupPage.Select(group => @group as Group)
                .Where(n => n.DisplayName.ToLowerInvariant().Contains(filter.ToLowerInvariant()));
            filteredList.AddRange(groups);

            while (groupPage.NextPageRequest != null)
            {
                groupPage = await groupPage.NextPageRequest.GetAsync();
                filteredList.AddRange(groupPage.Select(group => @group as Group)
                    .Where(n => n.DisplayName.ToLowerInvariant().Contains(filter.ToLowerInvariant())));
            }
            return defaultTop.HasValue
                ? filteredList.Take(defaultTop.Value).ToList()
                : filteredList.ToList();
        }

        public async Task<IList<Group>> GetGroupMembershipForUserNofilter(string userId, int? defaultTop = null)
        {
            var client = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All", "GroupMember.Read.All" });

            List<Group> filteredList = new List<Group>();
            var currentUser = client.Users[userId];
            var groupPage = await currentUser.MemberOf.Request().GetAsync();
            filteredList.AddRange(groupPage.Select(group => @group as Group));
            while (groupPage.NextPageRequest != null)
            {
                groupPage = await groupPage.NextPageRequest.GetAsync();
                filteredList.AddRange(groupPage.Select(group => @group as Group));
            }
            return defaultTop.HasValue
                ? filteredList.Take(defaultTop.Value).ToList()
                : filteredList.ToList();
        }

        public async Task<IList<User>> GetUserMembershipForGroup(string groupId, int? defaultTop = null)
        {
            var client = _graphServiceProvider.GetGraphServiceClient(new[] { "User.Read.All", "GroupMember.Read.All" });
            var currentGroup = client.Groups[groupId];
            var users = await currentGroup.Members.Request().GetAsync();
            return defaultTop.HasValue
                ? users.Select(user => user as User).Take(defaultTop.Value).ToList()
                : users.Select(user => user as User).ToList();
        }


    }

}

