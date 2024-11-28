import {SideBar} from "@/components/SideBar.tsx";
import { Outlet } from "react-router-dom";

function MainLayout() {

    return (
        <div className="flex w-screen h-screen">
            <SideBar className={"flex-grow-0"}/>
            <div className={"h-screen flex-grow overflow-hidden flex p-4"}>
                <Outlet />
            </div>
        </div>
    )
}

export default MainLayout