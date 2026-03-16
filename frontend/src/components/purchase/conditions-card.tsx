import { ShieldCheck } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { type Condition } from "@/features/properties/types/property";
import { badgeVariantForStatus, formatDate } from "@/components/purchase/utils";

function isClosedCondition(status: Condition["status"]) {
  return status === "satisfied" || status === "waived";
}

export function ConditionsCard({
  conditions,
  disabled,
  onCompleteCondition
}: {
  conditions: Condition[];
  disabled?: boolean;
  onCompleteCondition: (condition: Condition) => void;
}) {
  return (
    <Card className="rounded-2xl">
      <CardHeader>
        <div className="flex items-center gap-2">
          <ShieldCheck className="h-4 w-4 text-ink/70" />
          <CardTitle className="text-lg">Conditions</CardTitle>
        </div>
      </CardHeader>
      <CardContent className="space-y-3">
        {conditions.length === 0 ? <p className="text-sm text-ink/60">No active conditions for this purchase.</p> : null}
        {conditions.map((condition) => (
          <div key={condition.id} className="flex items-center justify-between gap-3 rounded-xl border border-line bg-white p-3">
            <div>
              <p className="text-sm font-medium">{condition.type}</p>
              <p className="text-sm text-ink/65">Due {formatDate(condition.dueDate)}</p>
            </div>
            <div className="flex items-center gap-2">
              <Badge variant={badgeVariantForStatus(condition.status)}>{condition.status}</Badge>
              <Button
                size="sm"
                variant="outline"
                className="rounded-lg"
                disabled={isClosedCondition(condition.status) || disabled}
                onClick={() => onCompleteCondition(condition)}
              >
                Complete
              </Button>
            </div>
          </div>
        ))}
      </CardContent>
    </Card>
  );
}
