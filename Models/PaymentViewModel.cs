namespace GymManagement.Models;

public class PaymentViewModel
{
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public int? MembershipId { get; set; }
    
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolder { get; set; } = string.Empty;
    public string Expiry { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
}
