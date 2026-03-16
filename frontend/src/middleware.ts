import { clerkMiddleware, createRouteMatcher } from "@clerk/nextjs/server";

const isProtectedRoute = createRouteMatcher(["/dashboard(.*)", "/properties(.*)"]);

export default clerkMiddleware(async (auth, req) => {
  const pathname = req.nextUrl.pathname;
  const isPublicCreateRoute = pathname === "/properties/new" || pathname === "/properties/new/";

  if (isProtectedRoute(req) && !isPublicCreateRoute) {
    await auth.protect();
  }
});

export const config = {
  matcher: ["/((?!_next|[^?]*\\.(?:html?|css|js(?!on)|jpe?g|png|gif|svg|ttf|woff2?|ico|csv|docx?|xlsx?|zip|webmanifest)).*)", "/(api|trpc)(.*)"]
};
