using System;
using System.Threading.Tasks;

using Grpc.Core;
using myungbae;  // gRPC 생성된 파일들 포함

public class StubClient
{
    private readonly Channel channel;
    private readonly GameService.GameServiceClient client;

    public StubClient(string host, int port)
    {
        // Grpc.Core의 Channel을 사용하여 GrpcChannel을 대체
        channel = new Channel(host, port, ChannelCredentials.Insecure); // HTTP/2 보안을 비활성화 (개발 환경에서만 사용 권장)
        client = new GameService.GameServiceClient(channel);
    }

    public void getUserInfo(int userid)
    {
        var request = new UserInfoRequest { Userid = userid };
        var response = client.getUserInfo(request);

        Console.WriteLine(response.Checkmessage);
        Console.WriteLine("User ID : " + response.Userid);
        Console.WriteLine("Nickname : " + response.Nkname);
        Console.WriteLine("Experience : " + response.Exp);
        Console.WriteLine("Save Time : " + response.Savetime);
    }

    public void saveUserInfo(int userid, string nkname, int exp, string savetime)
    {
        var request = new UserInfoRequest
        {
            Userid = userid,
            Nkname = nkname,
            Exp = exp,
            Savetime = savetime
        };

        var response = client.saveUserInfo(request);
        Console.WriteLine(response.Checkmessage);
    }

    public void getMapProgress(int userid)
    {
        var request = new MapProgressRequest { Userid = userid };
        var response = client.getMapProgress(request);

        Console.WriteLine(response.Checkmessage);
        Console.WriteLine("User ID : " + response.Userid);
        Console.WriteLine("User Location : " + response.Xloc + ", " + response.Yloc + ", " + response.Zloc);
        Console.WriteLine("Map Progress : " + response.Mapprogress);
    }

    public void saveMapProgress(int userid, int mapprogress, double xloc, double yloc, double zloc)
    {
        var request = new MapProgressRequest
        {
            Userid = userid,
            Mapprogress = mapprogress,
            Xloc = xloc,
            Yloc = yloc,
            Zloc = zloc
        };

        var response = client.saveMapProgress(request);
    }

    public void getItemRelation(int userid)
    {
        var request = new ItemRelationRequest { Userid = userid };
        var response = client.getItemRelation(request);

        Console.WriteLine(response.Checkmessage);
        Console.WriteLine("User ID : " + response.Userid);
        Console.WriteLine("Item ID : " + response.Itemid);
        Console.WriteLine("Item Name : " + response.Itemname);
        Console.WriteLine("Quantity : " + response.Quantity);
    }

    public void saveItemRelation(int userid, string itemid, int quantity)
    {
        var request = new ItemRelationRequest
        {
            Userid = userid,
            Itemid = itemid,
            Quantity = quantity
        };

        var response = client.saveItemRelation(request);
    }

    public async Task ShutdownAsync()
    {
        await channel.ShutdownAsync();
    }
}
