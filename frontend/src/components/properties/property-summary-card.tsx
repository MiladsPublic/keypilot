"use client";

import { useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useMutation } from "@tanstack/react-query";
import { useAuth } from "@clerk/nextjs";
import { CalendarClock, FileText, Plus, Trash2, Users } from "lucide-react";

import { completeCondition } from "@/features/properties/api/complete-condition";
import { completeTask } from "@/features/properties/api/complete-task";
import { settleProperty } from "@/features/properties/api/settle-property";
import { cancelProperty } from "@/features/properties/api/cancel-property";
import { waiveCondition } from "@/features/properties/api/waive-condition";
import { failCondition } from "@/features/properties/api/fail-condition";
import { addDocument, type AddDocumentBody } from "@/features/properties/api/add-document";
import { deleteDocument } from "@/features/properties/api/delete-document";
import { addContact, type AddContactBody } from "@/features/properties/api/add-contact";
import { deleteContact } from "@/features/properties/api/delete-contact";
import { type Condition, type Property, type PropertyTask } from "@/features/properties/types/property";
import { ConditionsCard, type ConditionAction } from "@/components/purchase/conditions-card";
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
  const groups = ["conditional", "unconditional", "pre_settlement", "settlement"] as const;

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

      <StageTimeline currentStatus={localProperty.status} />

      <div className="flex justify-end">
        <div className="flex flex-wrap justify-end gap-2">
          <Dialog open={cancelDialogOpen} onOpenChange={setCancelDialogOpen}>
            <DialogTrigger asChild>
              <Button className="rounded-full" variant="outline" disabled={localProperty.status === "settled" || localProperty.status === "cancelled"}>
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
              <Button className="rounded-full" disabled={localProperty.status === "settled" || localProperty.status === "cancelled"}>
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
            <TaskList groupedTasks={taskGroups} disabled={taskMutation.isPending} onToggleTask={markTaskComplete} />
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
          <TaskList groupedTasks={taskGroups} disabled={taskMutation.isPending} onToggleTask={markTaskComplete} />
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
