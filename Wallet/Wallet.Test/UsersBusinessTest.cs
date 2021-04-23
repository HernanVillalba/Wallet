using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Test
{
    class UsersBusinessTest
    {
    }
}

// TODO

//[Theory]
//[MemberData(nameof(Data_Paging))]
//public void GetByPage_PageExists_Ok(UserFilterModel filter, int page, int count)
//{
//    //Arrange
//    //Add users
//    context.Users.AddRange(usersList);
//    context.SaveChanges();
//    //Act
//    var result = usersController.GetUsersByPage(page, filter);
//    //Assert
//    Assert.IsType<OkObjectResult>(result);
//    var resultContent = (OkObjectResult)result;
//    //Test correct status code
//    Assert.Equal(200, resultContent.StatusCode);
//    //Test return type
//    Assert.IsAssignableFrom<IEnumerable<UserContact>>(resultContent.Value);
//    var resultList = (List<UserContact>)resultContent.Value;
//    //Test return count
//    Assert.Equal(count, resultList.Count);
//}
//public static IEnumerable<object[]> Data_Paging =>
//    new List<object[]>
//    {
//                //page 0 should return first page (10 first users)
//                new object[]
//                {
//                    new UserFilterModel(), 0, 10
//                },
//                //negative numbre page should return first page (10 first users)
//                new object[]
//                {
//                    new UserFilterModel(), Int32.MinValue, 10
//                },
//                //first page should return first ten users
//                new object[]
//                {
//                    new UserFilterModel(), 1, 10
//                },
//                //second page should return only one user
//                new object[]
//                {
//                    new UserFilterModel(), 2, 1
//                },
//                //first page filter by first name "A" should return two users
//                new object[] {
//                    new UserFilterModel
//                    {
//                        FirstName = "A",
//                        LastName = "",
//                        Email = ""
//                    }, 1, 2
//                },
//                //first page filter by first name "AA" and last name "AA" should return three users
//                new object[]
//                {
//                    new UserFilterModel
//                    {
//                        FirstName = "AA",
//                        LastName = "AA",
//                        Email = ""
//                    }, 1, 3
//                },
//                new object[]
//                {
//                    new UserFilterModel
//                    {
//                        FirstName = "",
//                        LastName = "",
//                        Email = "B@mail.com"
//                    }, 1, 1
//                }
//    };