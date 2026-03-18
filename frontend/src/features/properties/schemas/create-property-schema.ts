import { z } from "zod";

const requiredDate = z.string().regex(/^\d{4}-\d{2}-\d{2}$/, "Use YYYY-MM-DD");
const optionalMoney = z.string().regex(/^\d+(\.\d{1,2})?$/, "Use a valid amount").or(z.literal(""));

const buyingMethods = ["private_sale", "auction", "negotiation", "tender", "deadline"] as const;

export const createPropertySchema = z
  .object({
    address: z.string().trim().min(3, "Address is required"),
    buyingMethod: z.enum(buyingMethods, { message: "Select a buying method" }),
    acceptedOfferDate: requiredDate,
    settlementDate: requiredDate,
    purchasePrice: optionalMoney.optional(),
    depositAmount: optionalMoney.optional(),
    methodReference: z.string().max(100).optional(),
    conditions: z.object({
      finance: z.boolean(),
      building_report: z.boolean(),
      lim: z.boolean(),
      insurance: z.boolean(),
      solicitor_approval: z.boolean()
    })
  })
  .refine(
    (value) => {
      return value.settlementDate >= value.acceptedOfferDate;
    },
    {
      message: "Settlement date must be on or after the offer accepted date",
      path: ["settlementDate"]
    }
  );

export type CreatePropertyFormValues = z.infer<typeof createPropertySchema>;
