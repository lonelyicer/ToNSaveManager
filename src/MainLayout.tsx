import { SideBar } from "@/components/SideBar.tsx";
import { Outlet, useLocation } from "react-router-dom";
import { motion, AnimatePresence } from "framer-motion";

function MainLayout() {
    const location = useLocation();

    return (
        <div className="flex w-screen h-screen">
            <SideBar className={"flex-grow-0"} />
            <div className={"h-screen flex-grow overflow-hidden flex p-4 w-full"}>
                <AnimatePresence mode='wait'>
                    <motion.div
                        key={location.pathname}
                        initial={{ opacity: 0, y: 20 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.2 }} 
                        className={"w-full"}
                    >
                        <Outlet />
                    </motion.div>
                </AnimatePresence>
            </div>
        </div>
    );
}

export default MainLayout;
