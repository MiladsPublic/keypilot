import { z } from "zod";

const optionalDate = z.string().regex(/^\d{4}-\d{2}-\d{2}$/, "Use YYYY-MM-DD").or(z.literal(""));
const optionalMoney = z.string().regex(/^\d+(\.\d{1,2})?$/, "Use a valid amount").or(z.literal(""));

export const createPropertySchema = z
  .object({
    address: z.string().trim().min(3, "Address is required"),
    offerAcceptedDate: optionalDate.optional(),
    settlementDate: optionalDate.optional(),
    purchasePrice: optionalMoney.optional()
  })
  .refine(
    (value) => {
      if (!value.offerAcceptedDate || !value.settlementDate) {
        return true;
      }

      return value.settlementDate >= value.offerAcceptedDate;
    },
    {
      message: "Settlement date must be on or after the offer accepted date",
      path: ["settlementDate"]
    }
  );

export type CreatePropertyFormValues = z.infer<typeof createPropertySchema>;
