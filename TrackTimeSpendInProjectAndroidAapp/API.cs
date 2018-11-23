using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackTimeSpendInProjectAndroidAapp.Models;

namespace TrackTimeSpendInProjectAndroidAapp
{
    public class API
    {
        public async Task<List<MemberModel>> GetMembers()
        {
            try
            {
                RestClient Client = new RestClient("http://track.deveim.com/api/members/get");
                RestRequest request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteTaskAsync(request);

                if (response.IsSuccessful)
                {
                    var members = JsonConvert.DeserializeObject<List<MemberModel>>(response.Content);
                    return members;
                }
                else
                    return null;

            }
            catch (Exception e)
            {
                await SendError(new Error(e));
                return null;
            }
        }

        public async Task<RequestStatus> SetActive(MemberModel member)
        {
            try
            {
                RestClient Client = new RestClient("http://track.deveim.com/api/members/ChangeStatus");
                RestRequest request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", JsonConvert.SerializeObject(member), ParameterType.RequestBody);
                IRestResponse response = await Client.ExecuteTaskAsync(request);

                if (response.IsSuccessful)
                {
                    var requestStatus = JsonConvert.DeserializeObject<RequestStatus>(response.Content);
                    return requestStatus;
                }
                else
                    return new RequestStatus(false);
            }
            catch (Exception e)
            {
                await SendError(new Error(e));
                return new RequestStatus(e.Message, false); ;
            }
        }

        public async Task<RequestStatus> Work(bool status, MemberModel member)
        {
            try
            {
                RestClient Client = new RestClient();

                if (status)
                    Client.BaseUrl = new Uri("http://track.deveim.com/api/members/MemberStarted/" + DateTime.Now.ToString());
                else
                    Client.BaseUrl = new Uri("http://track.deveim.com/api/members/MemberStoped/" + DateTime.Now.ToString());

                RestRequest Request = new RestRequest(Method.POST);
                Request.AddHeader("Content-Type", "application/json");
                Request.AddParameter("undefined", JsonConvert.SerializeObject(member), ParameterType.RequestBody);
                IRestResponse response = await Client.ExecuteTaskAsync(Request);
                if (response.IsSuccessful)
                {
                    var RequestStatus = JsonConvert.DeserializeObject<RequestStatus>(response.Content);
                    return RequestStatus;
                }
                else
                    return new RequestStatus("Api Call error", false);
            }
            catch (Exception e)
            {
                return new RequestStatus(e.Message, false);
            }
        }

        public async Task<RequestStatus> SendError(Error error)
        {
            try
            {
                return new RequestStatus(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new RequestStatus(false);
            }
        }
    }
}