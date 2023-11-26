namespace QR_Ganize_Lib;

public abstract class Result<TSuccess, TError> { }

public sealed class Nothing {
    private Nothing() { }
    public static Nothing AtAll { get; } = new Nothing();
}

public class Ok<TSuccess, TError> : Result<TSuccess, TError>
{
    public TSuccess Data{get;}
    public Ok(TSuccess data)=>Data=data;
    public void Deconstruct(out TSuccess data)=>data=Data;
}

public class Err<TSuccess, TError> : Result<TSuccess, TError>
{
    public TError Error{get;}
    public Err(TError error)=>Error=error;
    public void Deconstruct(out TError error)=>error=Error;

}