using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pickflicks2.Models;
using pickflicks2.Models.DTO;
using pickflicks2.Services;
using pickflicks2.Services.Context;

namespace pickflicks2.Services
{
    public class MWGService
    {
        private readonly DataContext _context;
        public MWGService(DataContext context)
        {
            _context = context;
        }

        public bool AddMWG(MWGModel newMWGModel)
        {
            bool result = false;
            bool doesMWGExist = _context.MWGInfo.SingleOrDefault(MWG => MWG.Id == newMWGModel.Id) != null;
            if (!doesMWGExist)
            {
                _context.Add(newMWGModel);
                result = _context.SaveChanges() != 0;
            }
            return result;
        }

        public IEnumerable<MWGModel> GetAllMWG()
        {
            return _context.MWGInfo;
        }

        public MWGModel GetMWGById(int id)
        {
            return _context.MWGInfo.SingleOrDefault(item => item.Id == id);
        }

        public MWGModel GetMWGByMWGName(string MWGName)
        {
            return _context.MWGInfo.SingleOrDefault(item => item.MWGName == MWGName);
        }

        public IEnumerable<MWGModel> GetAllCreatedMWGByUserId(int userId)
        {
            return _context.MWGInfo.Where(item => item.GroupCreatorId == userId);
        }

        public List<MWGModel> GetAllMWGAUserIsMemberOf(int userId)
        {
            //"Tag1, Tag2, Tag3,Tag4"
            List<MWGModel> AllMWGWithMemberId = new List<MWGModel>();//[]
            var allMWG = GetAllMWG().ToList();//{Tag:"Tag1, Tag2",Tag:"Tag2",Tag:"tag3"}
            for (int i = 0; i < allMWG.Count; i++)
            {
                MWGModel Group = allMWG[i];//{Tag:"Tag1, Tag2"}
                var groupArr = Group.MembersId.Split(",");//["Tag1","Tag2"]
                for (int j = 0; j < groupArr.Length; j++)
                {   //Tag1 j = 0
                    //Tag2 j = 1
                    if (groupArr[j].Contains(userId.ToString()))
                    {// Tag1               Tag1
                        AllMWGWithMemberId.Add(Group);//{Tag:"Tag1, Tag2"}
                    }
                }
            }
            return AllMWGWithMemberId;
        }

        public bool EditMWGName(string? oldMWGName, string? updatedMWGName)
        {
            bool result = false;
            MWGModel foundMWG = GetMWGByMWGName(oldMWGName);
            if (foundMWG != null)
            {
                foundMWG.MWGName = updatedMWGName;
                _context.Update<MWGModel>(foundMWG);
                result = _context.SaveChanges() != 0;
            }
            return result;
        }

        // Hopefully this works
        public bool AddMemberToMWG(int MWGId, int newMemberId)
        {
            bool result = false;
            MWGModel foundMWG = GetMWGById(MWGId);
            if (foundMWG != null)
            {
                // Append the new userId into the string
                foundMWG.MembersId += ',' + newMemberId.ToString();
                _context.Update<MWGModel>(foundMWG);
                result = _context.SaveChanges() != 0;
            }
            return result;
        }

        public bool AddUserSuggestedMovies(int MWGId, string? newMovie)
        {
            bool result = false;
            MWGModel foundMWG = GetMWGById(MWGId);
            if (foundMWG != null)
            {
                // Append the new userId into the string
                foundMWG.UserSuggestedMovies += newMovie + ',';
                _context.Update<MWGModel>(foundMWG);
                result = _context.SaveChanges() != 0;
            }
            return result;
        }


        public bool DeleteMemberFromMWG(int MWGId, int deletedMemberId)
        {
            bool result = false;
            MWGModel foundMWG = GetMWGById(MWGId);
            if (foundMWG != null)
            {
                int position = foundMWG.MembersId.IndexOf(deletedMemberId.ToString());
                if (position == foundMWG.MembersId.Length - 1)
                {
                    foundMWG.MembersId = foundMWG.MembersId.Remove(position - 1, 2);

                }
                else
                {

                    foundMWG.MembersId = foundMWG.MembersId.Remove(position, 2);
                }
                _context.Update<MWGModel>(foundMWG);
                result = _context.SaveChanges() != 0;
            }
            return result;
        }

        public bool DeleteByMWGName(string? MWGName)
        {
            bool result = false;
            MWGModel foundMWG = GetMWGByMWGName(MWGName);
            if (foundMWG != null)
            {
                foundMWG.IsDeleted = !foundMWG.IsDeleted;
                _context.Update<MWGModel>(foundMWG);
                result = _context.SaveChanges() != 0;
            }
            return result;
        }

        public bool DeleteByMWGId(int MWGId)
        {
            bool result = false;
            MWGModel foundMWG = GetMWGById(MWGId);
            if (foundMWG != null)
            {
                foundMWG.IsDeleted = !foundMWG.IsDeleted;
                _context.Update<MWGModel>(foundMWG);
                result = _context.SaveChanges() != 0;
            }
            return result;
        }


        //use this for backend
        public bool EditMWG(MWGModel MWG)
        {
            _context.Update<MWGModel>(MWG);
            return _context.SaveChanges() != 0;
        }
    }
}