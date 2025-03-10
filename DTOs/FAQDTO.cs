namespace EStore.DTOs;

public class FAQDTO
{
    public string Question { get; set; }
    public string Answer { get; set; }
}
public class FAQUpdateDTO
{
    public int Id { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
}