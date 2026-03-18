"use client";

import { useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useMutation } from "@tanstack/react-query";
import { useAuth } from "@clerk/nextjs";
import { CalendarClock, FileText, Gavel, Pencil, Plus, ShieldCheck, Trash2, Users } from "lucide-react";

import { completeCondition } from "@/features/properties/api/complete-condition";
import { completeTask } from "@/features/properties/api/complete-task";
import { settleProperty } from "@/features/properties/api/settle-property";
import { cancelProperty } from "@/features/properties/api/cancel-property";
import { archiveProperty } from "@/features/properties/api/archive-property";
import { waiveCondition } from "@/features/properties/api/waive-condition";
import { failCondition } from "@/features/properties/api/fail-condition";
import { addDocument, type AddDocumentBody } from "@/features/properties/api/add-document";
import { deleteDocument } from "@/features/properties/api/delete-document";
import { addContact, type AddContactBody } from "@/features/properties/api/add-contact";
import { deleteContact } from "@/features/properties/api/delete-contact";
import { updateProperty, type UpdatePropertyBody } from "@/features/properties/api/update-property";
import { submitOffer, type SubmitOfferBody } from "@/features/properties/api/submit-offer";
import { goUnconditional } from "@/features/properties/api/go-unconditional";
import { type Condition, type Property, type PropertyTask } from "@/features/properties/types/property";
import { ConditionsCard, type ConditionAction } from "@/components/purchase/conditions-card";
import { MethodGuidanceBanner } from "@/components/purchase/method-guidance-banner";
import { ProgressCard } from "@/components/purchase/progress-card";
import { PurchaseHeroCard } from "@/components/purchase/purchase-hero-card";
import { StageTimeline } from "@/components/purchase/stage-timeline";
import { TaskList } from "@/components/purchase/task-list";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from "@/components/ui/dialog";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { toast } from "@/hooks/use-toast";

function groupedTasks(tasks: PropertyTask[]) {
  const groups = ["discovery", "offer_preparation", "submitted", "conditional", "unconditional", "settlement_pending", "settlement"] as const;

  return groups.map((stage) => ({
    stage,
    tasks: tasks.filter((task) => task.stage === stage)
  }));
}

export function PropertySummaryCard({ property }: { property: Property }) {
  const router = useRouter();
  const { getToken } = useAuth();

  const [settleDialogOpen, setSettleDialogOpen] = useState(false);
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);
  const [addDocDialogOpen, setAddDocDialogOpen] = useState(false);
  const [addContactDialogOpen, setAddContactDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [submitOfferDialogOpen, setSubmitOfferDialogOpen] = useState(false);
  const [goUnconditionalDialogOpen, setGoUnconditionalDialogOpen] = useState(false);
  const [auctionReadinessDialogOpen, setAuctionReadinessDialogOpen] = useState(false);
  const [localProperty, setLocalProperty] = useState(property);

  const taskMutation = useMutation({
    mutationFn: async (task: PropertyTask) => completeTask(task.id, await getToken()),
    onSuccess: (_data, task) => {
      toast({
        title: "Task completed",
        description: `Marked \"${task.title}\" as complete.`,
        variant: "success"
      });
      router.refresh();
    },
    onError: (_error, task) => {
      toast({
        title: "Couldn't update task",
        description: `Couldn't mark \"${task.title}\" as complete.`,
        variant: "danger"
      });
      router.refresh();
    }
  });

  const conditionMutation = useMutation({
    mutationFn: async ({ condition, action }: { condition: Condition; action: ConditionAction }) => {
      const token = await getToken();

      if (action === "waive") {
        return waiveCondition(condition.id, token);
      }

      if (action === "fail") {
        return failCondition(condition.id, token);
      }

      return completeCondition(condition.id, token);
    },
    onSuccess: (_data, { condition, action }) => {
      const actionLabel = action === "waive" ? "waived" : action === "fail" ? "failed" : "satisfied";
      toast({
        title: `Condition ${actionLabel}`,
        description: `Marked "${condition.type}" as ${actionLabel}.`,
        variant: "success"
      });
      router.refresh();
    },
    onError: (_error, { condition, action }) => {
      const actionLabel = action === "waive" ? "waived" : action === "fail" ? "failed" : "satisfied";
      toast({
        title: "Couldn't update condition",
        description: `Couldn't mark "${condition.type}" as ${actionLabel}.`,
        variant: "danger"
      });
      router.refresh();
    }
  });

  const settleMutation = useMutation({
    mutationFn: async (id: string) => settleProperty(id, await getToken()),
    onSuccess: () => {
      toast({
        title: "Purchase settled",
        description: `${localProperty.address} is now marked as settled.`,
        variant: "success"
      });
      setSettleDialogOpen(false);
      router.refresh();
    },
    onError: () => {
      toast({
        title: "Couldn't settle purchase",
        description: `Couldn't settle ${localProperty.address}.`,
        variant: "danger"
      });
    }
  });

  const cancelMutation = useMutation({
    mutationFn: async (id: string) => cancelProperty(id, await getToken()),
    onSuccess: () => {
      toast({
        title: "Purchase cancelled",
        description: `${localProperty.address} is now marked as cancelled.`,
        variant: "warning"
      });
      setCancelDialogOpen(false);
      router.refresh();
    },
    onError: () => {
      toast({
        title: "Couldn't cancel purchase",
        description: `Couldn't cancel ${localProperty.address}.`,
        variant: "danger"
      });
    }
  });

  const archiveMutation = useMutation({
    mutationFn: async (id: string) => archiveProperty(id, await getToken()),
    onSuccess: () => {
      toast({
        title: "Purchase archived",
        description: `${localProperty.address} has been archived.`,
        variant: "success"
      });
      router.refresh();
    },
    onError: () => {
      toast({
        title: "Couldn't archive purchase",
        description: `Couldn't archive ${localProperty.address}.`,
        variant: "danger"
      });
    }
  });

  const addDocumentMutation = useMutation({
    mutationFn: async (body: AddDocumentBody) => addDocument(localProperty.id, body, await getToken()),
    onSuccess: (doc) => {
      setLocalProperty((prev) => ({ ...prev, documents: [...prev.documents, doc] }));
      setAddDocDialogOpen(false);
      toast({ title: "Document added", description: doc.fileName, variant: "success" });
    },
    onError: () => {
      toast({ title: "Couldn't add document", variant: "danger" });
    }
  });

  const deleteDocumentMutation = useMutation({
    mutationFn: async (id: string) => { await deleteDocument(id, await getToken()); return id; },
    onSuccess: (id) => {
      setLocalProperty((prev) => ({ ...prev, documents: prev.documents.filter((d) => d.id !== id) }));
      toast({ title: "Document removed", variant: "success" });
    },
    onError: () => {
      toast({ title: "Couldn't remove document", variant: "danger" });
    }
  });

  const addContactMutation = useMutation({
    mutationFn: async (body: AddContactBody) => addContact(localProperty.id, body, await getToken()),
    onSuccess: (contact) => {
      setLocalProperty((prev) => ({ ...prev, contacts: [...prev.contacts, contact] }));
      setAddContactDialogOpen(false);
      toast({ title: "Contact added", description: contact.name, variant: "success" });
    },
    onError: () => {
      toast({ title: "Couldn't add contact", variant: "danger" });
    }
  });

  const deleteContactMutation = useMutation({
    mutationFn: async (id: string) => { await deleteContact(id, await getToken()); return id; },
    onSuccess: (id) => {
      setLocalProperty((prev) => ({ ...prev, contacts: prev.contacts.filter((c) => c.id !== id) }));
      toast({ title: "Contact removed", variant: "success" });
    },
    onError: () => {
      toast({ title: "Couldn't remove contact", variant: "danger" });
    }
  });

  const updatePropertyMutation = useMutation({
    mutationFn: async (body: UpdatePropertyBody) => updateProperty(localProperty.id, body, await getToken()),
    onSuccess: (updated) => {
      setLocalProperty(updated);
      setEditDialogOpen(false);
      toast({ title: "Property updated", description: updated.address, variant: "success" });
      router.refresh();
    },
    onError: () => {
      toast({ title: "Couldn't update property", variant: "danger" });
    }
  });

  const submitOfferMutation = useMutation({
    mutationFn: async (body: SubmitOfferBody) => submitOffer(localProperty.id, body, await getToken()),
    onSuccess: (updated) => {
      setLocalProperty(updated);
      setSubmitOfferDialogOpen(false);
      toast({ title: "Offer submitted", description: "Your workspace is now in conditional stage.", variant: "success" });
      router.refresh();
    },
    onError: () => {
      toast({ title: "Couldn't submit offer", variant: "danger" });
    }
  });

  const goUnconditionalMutation = useMutation({
    mutationFn: async () => goUnconditional(localProperty.id, await getToken()),
    onSuccess: (updated) => {
      setLocalProperty(updated);
      setGoUnconditionalDialogOpen(false);
      toast({ title: "Gone unconditional", description: `${updated.address} is now unconditional.`, variant: "success" });
      router.refresh();
    },
    onError: () => {
      toast({ title: "Couldn't go unconditional", variant: "danger" });
      router.refresh();
    }
  });

  const allTasks = useMemo(() => [...localProperty.tasks], [localProperty.tasks]);
  const taskGroups = groupedTasks(allTasks);

  const progressValue =
    localProperty.taskSummary.total === 0
      ? 0
      : Math.round((localProperty.taskSummary.completed / localProperty.taskSummary.total) * 100);

  const markTaskComplete = (task: PropertyTask) => {
    if (task.status === "completed") {
      return;
    }

    setLocalProperty((prev) => {
      const nextTasks = prev.tasks.map((item) =>
        item.id === task.id ? { ...item, status: "completed" as const, completedAtUtc: new Date().toISOString() } : item
      );

      const completed = nextTasks.filter((item) => item.status === "completed").length;

      return {
        ...prev,
        tasks: nextTasks,
        taskSummary: {
          total: nextTasks.length,
          completed,
          pending: nextTasks.length - completed
        }
      };
    });

    taskMutation.mutate(task);
  };

  const handleConditionAction = (condition: Condition, action: ConditionAction) => {
    if ((condition.status === "satisfied" || condition.status === "waived") && action !== "fail") {
      return;
    }

    const nextStatus: Condition["status"] =
      action === "waive" ? "waived" : action === "fail" ? "failed" : "satisfied";

    setLocalProperty((prev) => {
      const nextConditions = prev.conditions.map((item) =>
        item.id === condition.id
          ? {
              ...item,
              status: nextStatus,
              completedAtUtc: action === "fail" ? null : new Date().toISOString()
            }
          : item
      );

      return {
        ...prev,
        conditions: nextConditions
      };
    });

    conditionMutation.mutate({ condition, action });
  };

  return (
    <div className="space-y-6">
      <PurchaseHeroCard property={localProperty} progressValue={progressValue} />

      <StageTimeline currentStatus={localProperty.status} buyingMethod={localProperty.buyingMethod} />

      <MethodGuidanceBanner property={localProperty} />

      <div className="flex justify-end">
        <div className="flex flex-wrap justify-end gap-2">
          {(localProperty.status === "settled" || localProperty.status === "cancelled") ? (
            <Button
              className="rounded-full"
              variant="outline"
              disabled={archiveMutation.isPending}
              onClick={() => archiveMutation.mutate(localProperty.id)}
            >
              Archive
            </Button>
          ) : null}

          {localProperty.status !== "settled" && localProperty.status !== "cancelled" && localProperty.status !== "archived" ? (
            <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
              <DialogTrigger asChild>
                <Button className="rounded-full" variant="outline">
                  <Pencil className="mr-1 h-4 w-4" /> Edit details
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Edit property details</DialogTitle>
                  <DialogDescription>Update the details for this purchase.</DialogDescription>
                </DialogHeader>
                <form
                  onSubmit={(e) => {
                    e.preventDefault();
                    const data = new FormData(e.currentTarget);
                    const body: UpdatePropertyBody = {};
                    const address = (data.get("address") as string)?.trim();
                    if (address && address !== localProperty.address) body.address = address;
                    const settlement = data.get("settlementDate") as string;
                    if (settlement && settlement !== localProperty.settlementDate) body.settlementDate = settlement;
                    const price = data.get("purchasePrice") as string;
                    if (price) body.purchasePrice = Number(price);
                    const deposit = data.get("depositAmount") as string;
                    if (deposit) body.depositAmount = Number(deposit);
                    const methodRef = (data.get("methodReference") as string)?.trim();
                    if (methodRef !== (localProperty.methodReference ?? "")) body.methodReference = methodRef || null;
                    if (Object.keys(body).length > 0) updatePropertyMutation.mutate(body);
                    else setEditDialogOpen(false);
                  }}
                  className="space-y-4"
                >
                  <div>
                    <label htmlFor="edit-address" className="mb-1 block text-sm font-medium">Address</label>
                    <Input id="edit-address" name="address" defaultValue={localProperty.address} />
                  </div>
                  <div>
                    <label htmlFor="edit-settlement" className="mb-1 block text-sm font-medium">Settlement date</label>
                    <Input id="edit-settlement" name="settlementDate" type="date" defaultValue={localProperty.settlementDate ?? ""} />
                  </div>
                  <div>
                    <label htmlFor="edit-price" className="mb-1 block text-sm font-medium">Purchase price</label>
                    <Input id="edit-price" name="purchasePrice" type="number" step="0.01" defaultValue={localProperty.purchasePrice ?? ""} />
                  </div>
                  <div>
                    <label htmlFor="edit-deposit" className="mb-1 block text-sm font-medium">Deposit amount</label>
                    <Input id="edit-deposit" name="depositAmount" type="number" step="0.01" defaultValue={localProperty.depositAmount ?? ""} />
                  </div>
                  <div>
                    <label htmlFor="edit-methodRef" className="mb-1 block text-sm font-medium">Method reference</label>
                    <Input id="edit-methodRef" name="methodReference" defaultValue={localProperty.methodReference ?? ""} placeholder="e.g. auction lot number" />
                  </div>
                  <DialogFooter>
                    <Button variant="outline" className="rounded-lg" type="button" onClick={() => setEditDialogOpen(false)}>Cancel</Button>
                    <Button type="submit" className="rounded-full" disabled={updatePropertyMutation.isPending}>Save changes</Button>
                  </DialogFooter>
                </form>
              </DialogContent>
            </Dialog>
          ) : null}

          {localProperty.status === "discovery" ? (
            <Dialog open={submitOfferDialogOpen} onOpenChange={setSubmitOfferDialogOpen}>
              <DialogTrigger asChild>
                <Button className="rounded-full">Submit offer</Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Submit an offer</DialogTitle>
                  <DialogDescription>Provide the accepted offer details to move into the conditional stage.</DialogDescription>
                </DialogHeader>
                <form
                  onSubmit={(e) => {
                    e.preventDefault();
                    const data = new FormData(e.currentTarget);
                    const offsets: Record<string, number> = { finance: 5, building_report: 5, lim: 10, insurance: 10, solicitor_approval: 5 };
                    const conditions = ["finance", "building_report", "lim", "insurance", "solicitor_approval"]
                      .filter((type) => data.get(`cond-${type}`) === "on")
                      .map((type) => ({ type, daysFromAcceptedOffer: offsets[type] ?? 5 }));
                    submitOfferMutation.mutate({
                      acceptedOfferDate: data.get("acceptedOfferDate") as string,
                      settlementDate: data.get("settlementDate") as string,
                      conditions: conditions.length > 0 ? conditions : undefined
                    });
                  }}
                  className="space-y-4"
                >
                  <div>
                    <label htmlFor="offer-accepted" className="mb-1 block text-sm font-medium">Offer accepted date</label>
                    <Input id="offer-accepted" name="acceptedOfferDate" type="date" required />
                  </div>
                  <div>
                    <label htmlFor="offer-settlement" className="mb-1 block text-sm font-medium">Settlement date</label>
                    <Input id="offer-settlement" name="settlementDate" type="date" required />
                  </div>
                  <fieldset className="space-y-2">
                    <legend className="text-sm font-medium">Conditions</legend>
                    {[
                      { value: "finance", label: "Finance" },
                      { value: "building_report", label: "Building report" },
                      { value: "lim", label: "LIM" },
                      { value: "insurance", label: "Insurance" },
                      { value: "solicitor_approval", label: "Solicitor approval" }
                    ].map((cond) => (
                      <label key={cond.value} className="flex items-center gap-2 text-sm">
                        <input type="checkbox" name={`cond-${cond.value}`} defaultChecked={cond.value === "finance" || cond.value === "building_report"} />
                        {cond.label}
                      </label>
                    ))}
                  </fieldset>
                  <DialogFooter>
                    <Button variant="outline" className="rounded-lg" type="button" onClick={() => setSubmitOfferDialogOpen(false)}>Cancel</Button>
                    <Button type="submit" className="rounded-full" disabled={submitOfferMutation.isPending}>Submit offer</Button>
                  </DialogFooter>
                </form>
              </DialogContent>
            </Dialog>
          ) : null}

          {localProperty.status === "conditional" ? (
            <Dialog open={goUnconditionalDialogOpen} onOpenChange={setGoUnconditionalDialogOpen}>
              <DialogTrigger asChild>
                <Button className="rounded-full" variant="outline">
                  <ShieldCheck className="mr-1 h-4 w-4" /> Go unconditional
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Go unconditional?</DialogTitle>
                  <DialogDescription>
                    This confirms that all conditions are satisfied and you are committing to the purchase. Once unconditional, the purchase cannot be reversed without legal consequences.
                  </DialogDescription>
                </DialogHeader>
                <DialogFooter>
                  <Button variant="outline" className="rounded-lg" onClick={() => setGoUnconditionalDialogOpen(false)}>Cancel</Button>
                  <Button
                    className="rounded-full"
                    disabled={goUnconditionalMutation.isPending}
                    onClick={() => goUnconditionalMutation.mutate()}
                  >
                    Confirm unconditional
                  </Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>
          ) : null}

          {localProperty.buyingMethod === "auction" && (localProperty.status === "discovery" || localProperty.status === "offer_preparation") ? (() => {
            const incompleteTasks = localProperty.tasks.filter((t) => t.status === "pending");
            const isReady = incompleteTasks.length === 0;
            return (
              <Dialog open={auctionReadinessDialogOpen} onOpenChange={setAuctionReadinessDialogOpen}>
                <DialogTrigger asChild>
                  <Button className="rounded-full">
                    <Gavel className="mr-1 h-4 w-4" /> Auction readiness check
                  </Button>
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Are you ready to bid?</DialogTitle>
                    <DialogDescription>
                      Auction purchases are unconditional — once the hammer falls, you are legally committed. Review your preparation status below.
                    </DialogDescription>
                  </DialogHeader>
                  {isReady ? (
                    <p className="text-sm font-medium text-[var(--success-fg)]">All preparation tasks are complete. You are ready to bid.</p>
                  ) : (
                    <div className="space-y-2">
                      <p className="text-sm font-medium text-[var(--danger-fg)]">{incompleteTasks.length} task{incompleteTasks.length !== 1 ? "s" : ""} still incomplete:</p>
                      <ul className="space-y-1">
                        {incompleteTasks.slice(0, 8).map((task) => (
                          <li key={task.id} className="text-sm text-ink/75">• {task.title}</li>
                        ))}
                        {incompleteTasks.length > 8 ? <li className="text-sm text-ink/55">… and {incompleteTasks.length - 8} more</li> : null}
                      </ul>
                    </div>
                  )}
                  <DialogFooter>
                    <Button variant="outline" className="rounded-lg" onClick={() => setAuctionReadinessDialogOpen(false)}>Close</Button>
                  </DialogFooter>
                </DialogContent>
              </Dialog>
            );
          })() : null}

          <Dialog open={cancelDialogOpen} onOpenChange={setCancelDialogOpen}>
            <DialogTrigger asChild>
              <Button className="rounded-full" variant="outline" disabled={localProperty.status === "settled" || localProperty.status === "cancelled" || localProperty.status === "archived"}>
                Mark as cancelled
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Cancel this purchase?</DialogTitle>
                <DialogDescription>This will set the purchase stage to cancelled.</DialogDescription>
              </DialogHeader>
              <DialogFooter>
                <Button variant="outline" className="rounded-lg" onClick={() => setCancelDialogOpen(false)}>
                  Keep purchase
                </Button>
                <Button
                  className="rounded-full"
                  disabled={cancelMutation.isPending}
                  onClick={() => cancelMutation.mutate(localProperty.id)}
                >
                  Confirm cancel
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>

          <Dialog open={settleDialogOpen} onOpenChange={setSettleDialogOpen}>
            <DialogTrigger asChild>
              <Button className="rounded-full" disabled={localProperty.status === "settled" || localProperty.status === "cancelled" || localProperty.status === "archived"}>
                Mark as settled
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Mark this purchase as settled?</DialogTitle>
                <DialogDescription>This will set the purchase stage to settled.</DialogDescription>
              </DialogHeader>
              <DialogFooter>
                <Button variant="outline" className="rounded-lg" onClick={() => setSettleDialogOpen(false)}>
                  Cancel
                </Button>
                <Button
                  className="rounded-full"
                  disabled={settleMutation.isPending}
                  onClick={() => settleMutation.mutate(localProperty.id)}
                >
                  Confirm
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </div>
      </div>

      <Tabs defaultValue="overview">
        <TabsList>
          <TabsTrigger value="overview">Overview</TabsTrigger>
          <TabsTrigger value="tasks">Tasks</TabsTrigger>
          <TabsTrigger value="conditions">Conditions</TabsTrigger>
          <TabsTrigger value="documents">Documents</TabsTrigger>
          <TabsTrigger value="contacts">Contacts</TabsTrigger>
        </TabsList>

        <TabsContent value="overview" className="grid gap-6 lg:grid-cols-3">
          <div className="lg:col-span-2">
            <TaskList groupedTasks={taskGroups} buyingMethod={localProperty.buyingMethod} disabled={taskMutation.isPending} onToggleTask={markTaskComplete} />
          </div>
          <div>
            <ProgressCard
              completed={localProperty.taskSummary.completed}
              total={localProperty.taskSummary.total}
              pending={localProperty.taskSummary.pending}
              openConditions={localProperty.readinessSummary.openConditions}
              readinessSummary={localProperty.readinessSummary}
            />
          </div>
        </TabsContent>

        <TabsContent value="tasks">
          <TaskList groupedTasks={taskGroups} buyingMethod={localProperty.buyingMethod} disabled={taskMutation.isPending} onToggleTask={markTaskComplete} />
        </TabsContent>

        <TabsContent value="conditions">
          <ConditionsCard
            conditions={localProperty.conditions}
            disabled={conditionMutation.isPending}
            onConditionAction={handleConditionAction}
          />
        </TabsContent>

        <TabsContent value="documents">
          <div className="rounded-2xl border border-line bg-white p-6">
            <div className="flex items-center justify-between">
              <p className="inline-flex items-center gap-2 text-lg font-semibold">
                <FileText className="h-5 w-5" />
                Documents ({localProperty.documents.length})
              </p>
              <Dialog open={addDocDialogOpen} onOpenChange={setAddDocDialogOpen}>
                <DialogTrigger asChild>
                  <Button size="sm" variant="outline" className="rounded-full">
                    <Plus className="mr-1 h-4 w-4" /> Add
                  </Button>
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Add document</DialogTitle>
                    <DialogDescription>Record a document reference for this property.</DialogDescription>
                  </DialogHeader>
                  <form
                    onSubmit={(e) => {
                      e.preventDefault();
                      const form = e.currentTarget;
                      const data = new FormData(form);
                      addDocumentMutation.mutate({
                        storageKey: data.get("storageKey") as string,
                        fileName: data.get("fileName") as string,
                        category: data.get("category") as string
                      });
                    }}
                    className="space-y-4"
                  >
                    <div>
                      <label htmlFor="doc-fileName" className="mb-1 block text-sm font-medium">File name</label>
                      <Input id="doc-fileName" name="fileName" required placeholder="contract-v2.pdf" />
                    </div>
                    <div>
                      <label htmlFor="doc-category" className="mb-1 block text-sm font-medium">Category</label>
                      <Input id="doc-category" name="category" required placeholder="contract" />
                    </div>
                    <div>
                      <label htmlFor="doc-storageKey" className="mb-1 block text-sm font-medium">Storage key</label>
                      <Input id="doc-storageKey" name="storageKey" required placeholder="uploads/contract-v2.pdf" />
                    </div>
                    <DialogFooter>
                      <Button type="submit" className="rounded-full" disabled={addDocumentMutation.isPending}>
                        Add document
                      </Button>
                    </DialogFooter>
                  </form>
                </DialogContent>
              </Dialog>
            </div>

            {localProperty.documents.length === 0 ? (
              <p className="mt-4 text-sm text-ink/70">No documents yet.</p>
            ) : (
              <ul className="mt-4 divide-y divide-line">
                {localProperty.documents.map((doc) => (
                  <li key={doc.id} className="flex items-center justify-between py-3">
                    <div>
                      <p className="text-sm font-medium">{doc.fileName}</p>
                      <p className="text-xs text-ink/60">{doc.category}</p>
                    </div>
                    <Button
                      size="sm"
                      variant="ghost"
                      className="text-ink/50 hover:text-red-600"
                      disabled={deleteDocumentMutation.isPending}
                      onClick={() => deleteDocumentMutation.mutate(doc.id)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </TabsContent>

        <TabsContent value="contacts">
          <div className="rounded-2xl border border-line bg-white p-6">
            <div className="flex items-center justify-between">
              <p className="inline-flex items-center gap-2 text-lg font-semibold">
                <Users className="h-5 w-5" />
                Contacts ({localProperty.contacts.length})
              </p>
              <Dialog open={addContactDialogOpen} onOpenChange={setAddContactDialogOpen}>
                <DialogTrigger asChild>
                  <Button size="sm" variant="outline" className="rounded-full">
                    <Plus className="mr-1 h-4 w-4" /> Add
                  </Button>
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Add contact</DialogTitle>
                    <DialogDescription>Add a contact related to this property.</DialogDescription>
                  </DialogHeader>
                  <form
                    onSubmit={(e) => {
                      e.preventDefault();
                      const form = e.currentTarget;
                      const data = new FormData(form);
                      addContactMutation.mutate({
                        role: data.get("role") as string,
                        name: data.get("name") as string,
                        email: (data.get("email") as string) || null,
                        phone: (data.get("phone") as string) || null
                      });
                    }}
                    className="space-y-4"
                  >
                    <div>
                      <label htmlFor="contact-name" className="mb-1 block text-sm font-medium">Name</label>
                      <Input id="contact-name" name="name" required placeholder="Jane Smith" />
                    </div>
                    <div>
                      <label htmlFor="contact-role" className="mb-1 block text-sm font-medium">Role</label>
                      <Input id="contact-role" name="role" required placeholder="solicitor" />
                    </div>
                    <div>
                      <label htmlFor="contact-email" className="mb-1 block text-sm font-medium">Email</label>
                      <Input id="contact-email" name="email" type="email" placeholder="jane@example.com" />
                    </div>
                    <div>
                      <label htmlFor="contact-phone" className="mb-1 block text-sm font-medium">Phone</label>
                      <Input id="contact-phone" name="phone" type="tel" placeholder="+64 21 123 4567" />
                    </div>
                    <DialogFooter>
                      <Button type="submit" className="rounded-full" disabled={addContactMutation.isPending}>
                        Add contact
                      </Button>
                    </DialogFooter>
                  </form>
                </DialogContent>
              </Dialog>
            </div>

            {localProperty.contacts.length === 0 ? (
              <p className="mt-4 text-sm text-ink/70">No contacts yet.</p>
            ) : (
              <ul className="mt-4 divide-y divide-line">
                {localProperty.contacts.map((contact) => (
                  <li key={contact.id} className="flex items-center justify-between py-3">
                    <div>
                      <p className="text-sm font-medium">{contact.name}</p>
                      <p className="text-xs text-ink/60">
                        {contact.role}
                        {contact.email ? ` · ${contact.email}` : ""}
                        {contact.phone ? ` · ${contact.phone}` : ""}
                      </p>
                    </div>
                    <Button
                      size="sm"
                      variant="ghost"
                      className="text-ink/50 hover:text-red-600"
                      disabled={deleteContactMutation.isPending}
                      onClick={() => deleteContactMutation.mutate(contact.id)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </TabsContent>
      </Tabs>

      <div className="rounded-2xl border border-line bg-white p-6 text-sm text-ink/70">
        <p className="inline-flex items-center gap-2 font-medium text-ink/80">
          <CalendarClock className="h-4 w-4" />
          Keep this workspace up to date as conditions and tasks are completed.
        </p>
      </div>
    </div>
  );
}
