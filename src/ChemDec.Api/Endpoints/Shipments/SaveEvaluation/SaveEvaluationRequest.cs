namespace ChemDec.Api.Endpoints.Shipments.SaveEvaluation;

public sealed record SaveEvaluationRequest
{
    public bool? EvalAmountOk { get; set; }
    public bool? EvalBiocidesOk { get; set; }
    public bool? EvalCapacityOk { get; set; }
    public bool? EvalContaminationRisk { get; set; }
    public string EvalEnvImpact { get; set; }
    public string EvalComments { get; set; }
}
